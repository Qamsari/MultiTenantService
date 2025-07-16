using GoldenVitrin.NginxManager.Services.Nginx.Business;

namespace GoldenVitrin.NginxManager.Services.Nginx.Interfaces;

public interface INginxManager {
    Task<string> TryQuitNginxAsync(CancellationToken cancellationToken);
    Task<string> TryReloadNginxAsync(CancellationToken cancellationToken);
    Task<string> TryStartNginxAsync(CancellationToken cancellationToken);
    Task<string> TryGetDomainSetting(string domainName, CancellationToken cancellationToken);
    Task<string> TryAddOrUpdateDomainAsync(DomainSettings domainSettings,CancellationToken cancellationToken);
}
