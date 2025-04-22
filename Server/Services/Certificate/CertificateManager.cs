using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Services.DataService;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server.Services.Certificate
{
    public class CertificateManagerOptions
    {
        public string Path { get; set; }
    }

    public interface ICertificateManager
    {
        X509Certificate2? GetHostRelatedCertificate(string host);
    }

    public class CertificateManager : ICertificateManager
    {
        private readonly ILogger<CertificateManager> _logger;
        private Dictionary<string, X509Certificate2> _storage = new CaseInsensitiveDictionary<X509Certificate2>();
        public CertificateManager(IOptions<CertificateManagerOptions> options,ILogger<CertificateManager> logger)
        {
            _logger = logger;
            //foreach (var certificatePath in Directory.GetFiles(options.Value.Path, "*.pem"))
            //{
            //    try
            //    {
            //        var host = Path.GetFileNameWithoutExtension(certificatePath).ToLower();
            //        var privateKeyPath = $"{Path.GetDirectoryName(certificatePath)}\\{host}.key";
            //        var cert = CreateCertificateFromPem(certificatePath, privateKeyPath,host);
            //        _logger.LogInformation("Certificate for {Host} loaded... ({f1},{f2})", host,certificatePath,privateKeyPath);
            //        _storage.Add(host, cert);
            //        _storage.Add($"www.{host}", cert);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Error in load certificate for {Path}", certificatePath);
            //    }
            //}

            foreach (var certificatePath in Directory.GetFiles(options.Value.Path, "*.pfx"))
            {
                try
                {
                    var host = Path.GetFileNameWithoutExtension(certificatePath).ToLower();
                    var cert = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, host);
                    _logger.LogInformation("Certificate for {Host} loaded from {f1}", host, certificatePath);
                    _storage.Add(host, cert);
                    _storage.Add($"www.{host}", cert);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in load certificate for {Path}", certificatePath);
                }
            }
        }

        public X509Certificate2? GetHostRelatedCertificate(string host)
        {
            _logger.LogInformation("{0}",_storage.LongCount());
            foreach (var item in _storage.Keys)
            {
                _logger.LogInformation(item);
            }
           return  _storage.TryGetValue(host.ToLower(), out var cert) ? cert : null;
        }

        //private X509Certificate2 CreateCertificateFromPem(string certificatePath, string privateKeyPath, string host)
        //{
        //    var certificate = new X509Certificate2(certificatePath);
        //    var rsa = RSA.Create();
        //    var keyContent = File.ReadAllText(privateKeyPath);
        //    rsa.ImportFromPem(keyContent);
        //    var cspParameters = new CspParameters()
        //    {
        //        KeyContainerName = host,
        //        Flags = CspProviderFlags.UseNonExportableKey,
        //    };
        //    var rsaPersistent = new RSACryptoServiceProvider(cspParameters);
        //    rsaPersistent.ImportParameters(rsa.ExportParameters(true));
        //    rsa.Dispose();
        //    rsa = rsaPersistent;

        //    return certificate.CopyWithPrivateKey(rsa);
    
        //}

    }
        public static class Setup
        {
            public static IServiceCollection AddCertificateManager(this IServiceCollection services, IConfiguration configuration)
                => services
                .Configure<CertificateManagerOptions>(configuration.GetSection("Certificate"))
                .AddSingleton < ICertificateManager,CertificateManager>();
        }
}
public class CaseInsensitiveDictionary<T> : Dictionary<string, T>
{
    public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {

    }
}