using GoldenVitrin.NginxManager.Services.Nginx.Interfaces;
using GoldenVitrin.NginxManager.Services.Nginx.Models;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GoldenVitrin.NginxManager.Services.Nginx.Business;
public class Manager(ILogger<Manager> logger, IOptions<ManagerOptions> options, CommandFactory commandFactory) : INginxManager
{
    private readonly ILogger<Manager> _logger = logger;
    private readonly CommandFactory _commandFactory = commandFactory;

    public async Task<string> TryStartNginxAsync(CancellationToken cancellationToken)
        => await TryRunCommand(_commandFactory.CresteNginxStart(), cancellationToken).ConfigureAwait(false);

    public async Task<string> TryQuitNginxAsync(CancellationToken cancellationToken)
            => await TryRunCommand(_commandFactory.CresteNginxQuit(), cancellationToken).ConfigureAwait(false);

    public async Task<string> TryReloadNginxAsync(CancellationToken cancellationToken)
        => await TryRunCommand(_commandFactory.CresteNginxReload(), cancellationToken).ConfigureAwait(false);

    //public async Task<string>
    private async Task<string> TryRunCommand(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
    {
        Process process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        return output;
    }

    private string GetDomainConfigFilePath(string domainName)
        => Path.Combine(options.Value.NginxConfigDirectoryPath, $"{domainName.ToLower()}.conf");
    public async Task<string> TryGetDomainSetting(string domainName, CancellationToken cancellationToken)
    {
        var filePath = GetDomainConfigFilePath(domainName);
        return File.Exists(filePath) ? await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false) : string.Empty;
    }

    public async 

    public async Task<string> TryAddOrUpdateDomainAsync(DomainSettings domainSettings, CancellationToken cancellationToken)
    {
        var retVal = string.Empty;
        var filePath = GetDomainConfigFilePath(domainSettings.DomainName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        try
        {
            var template = string.Empty;
            object? settings = null;

            template = NGINX_HTTP_TEMPLATE;
            settings = new { Domain = domainSettings.DomainName, certbotTempPath = options.Value.CertbotTempPath };

            var fileContent = FillTemplateBySettings(template, settings);
            await File.WriteAllTextAsync(filePath, fileContent, cancellationToken).ConfigureAwait(false);
            await this.TryReloadNginxAsync(cancellationToken).ConfigureAwait(false);




            if (domainSettings.HasSSL)
            {
                template = NGINX_SSL_TEMPLATE;
                settings = new { Domain = domainSettings };
            }
            else
            {
                
            }
            var fileContent = FillTemplateBySettings(template, settings);
            await File.WriteAllTextAsync(filePath, fileContent,cancellationToken).ConfigureAwait(false);
            retVal = fileContent;
            await this.TryReloadNginxAsync(cancellationToken).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            retVal = ex.ToString();
        }
        return retVal;
    }




    private string FillTemplateBySettings(string template, object settings)
    {

        string pattern = @"@settings\.(\w+)";
        return Regex.Replace(template, pattern, match =>
        {
            string propName = match.Groups[1].Value;
            PropertyInfo prop = settings.GetType().GetProperty(propName)!;
            return prop?.GetValue(settings)?.ToString() ?? "NULL";
        });

    }

    const string NGINX_HTTP_TEMPLATE = """
#{Domain:@setting.Domain,Ssl:false}
server {
    listen 80;
    server_name @settings.Domain;

    location /.well-known/acme-challenge/ {
        root @settings.certbotTempPath;
        allow all;
    }

    location / {
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_no_cache 1;
        proxy_cache_bypass 1;
    }
}
""";

    const string NGINX_SSL_TEMPLATE = """
#{Domain:@setting.Domain,Ssl:false,CertPath :'@settings.CertPath', KeyPath :'@settings.KeyPath', ChainPath :'@settings.ChainPath' }
server {
    listen 80;
    server_name @settings.Domain;
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl;
    server_name @settings.Domain;

    ssl_certificate @settings.CertPath;
    ssl_certificate_key @settings.KeyPath;
    ssl_trusted_certificate @settings.ChainPath;

    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers 'TLS_AES_128_GCM_SHA256:TLS_AES_256_GCM_SHA384:TLS_CHACHA20_20POLY1305_SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384';
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 1d;
    ssl_session_tickets off;
    ssl_stapling on;
    ssl_stapling_verify on;
    resolver 8.8.8.8 8.8.4.4 valid=300s;
    resolver_timeout 5s;
    add_header Strict-Transport-Security "max-age=63072000; includeSubDomains; preload" always;
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    add_header X-XSS-Protection "1; mode=block";

    location / {
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_no_cache 1;
        proxy_cache_bypass 1;
    }
}
""";
}



public class DomainSettings
{
    public string DomainName { get; set; }
    public int VendorId{ get; set; }
}

public class CommandFactory
{
    public ProcessStartInfo CresteNginxStart()
        => GetNginxBaseProcessStartInfo("start nginx");

    public ProcessStartInfo CresteNginxQuit()
        => GetNginxBaseProcessStartInfo("./nginx -s quit");

    public ProcessStartInfo CresteNginxReload()
        => GetNginxBaseProcessStartInfo("./nginx -s reload");

    private ProcessStartInfo GetNginxBaseProcessStartInfo(string command)
    {
        // Set up the process start info
        ProcessStartInfo processInfo = new ProcessStartInfo("powershell.exe", command);
        processInfo.WorkingDirectory = "C:\\nginx-1.28.0\\";
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        return processInfo;
    }

    private ProcessStartInfo GetCertbotProcessInfo()
    {
        new ProcessStartInfo
        {
            FileName = "certbot",
            Arguments = string.Join( " " ,["certonly","--webroot","-w","C:/nginx-1.28.0/www/bitbug/certbot","-d","www.bitbug.ir","--non-interactive","--agree-tos","--email","c3d_lover@yahoo.com","--logs-dir","C:/nginx-1.28.0/www/bitbug/logs","--debug","-v"]),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };


        ProcessStartInfo processInfo = new ProcessStartInfo("certbot","");
        processInfo.RedirectStandardOutput = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        return processInfo;
    }
}