namespace GoldenVitrin.NginxManager.Services.Nginx.Interfaces;

public interface INginxManager {
    Task<string> TryQuitNginxAsync(CancellationToken cancellationToken);
    Task<string> TryReloadNginxAsync(CancellationToken cancellationToken);
    Task<string> TryStartNginxAsync(CancellationToken cancellationToken);
}
