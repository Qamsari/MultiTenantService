using GoldenVitrin.NginxManager.Services.Nginx.Interfaces;
using GoldenVitrin.NginxManager.Services.Nginx.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GoldenVitrin.NginxManager.Services.Nginx.Business;
public class Manager(ILogger<Manager> logger, IOptions<ManagerOptions> options, CommandFactory commandFactory) : INginxManager
{
    private readonly ILogger<Manager> _logger = logger;
    private readonly CommandFactory _commandFactory = commandFactory;
    private readonly ManagerOptions _options = options.Value;

    public async Task<string> TryStartNginxAsync(CancellationToken cancellationToken)
        => await TryRunCommand(_commandFactory.CresteNginxStart(), cancellationToken).ConfigureAwait(false);

    public async Task<string> TryQuitNginxAsync(CancellationToken cancellationToken)
            => await TryRunCommand(_commandFactory.CresteNginxQuit(), cancellationToken).ConfigureAwait(false);

    public async Task<string> TryReloadNginxAsync(CancellationToken cancellationToken)
        => await TryRunCommand(_commandFactory.CresteNginxReload(), cancellationToken).ConfigureAwait(false);
    private async Task<string> TryRunCommand(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
    {
        Process process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        return output;
    }
}


public class CommandFactory
{
    public ProcessStartInfo CresteNginxStart()
        => GetProcessStartInfo("start nginx");

    public ProcessStartInfo CresteNginxQuit()
        => GetProcessStartInfo("./nginx -s quit");

    public ProcessStartInfo CresteNginxReload()
        => GetProcessStartInfo("./nginx -s reload");

    private ProcessStartInfo GetProcessStartInfo(string command)
    {
        // Set up the process start info
        ProcessStartInfo processInfo = new ProcessStartInfo("powershell.exe", command);
        processInfo.WorkingDirectory = "C:\\nginx-1.27.5\\";
        processInfo.RedirectStandardOutput = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        return processInfo;

    }
}