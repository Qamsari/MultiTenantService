using GoldenVitrin.NginxManager.Services.Nginx.Business;
using GoldenVitrin.NginxManager.Services.Nginx.Interfaces;
using GoldenVitrin.NginxManager.Services.Nginx.Models;

namespace GoldenVitrin.NginxManager.Services.Nginx;

public static class Setup
{
    public static IServiceCollection AddNginxManager(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddSingleton<INginxManager, Manager>()
            .AddSingleton<CommandFactory>()
            .AddOptions<ManagerOptions>().Bind(configuration);
        return services;
    }

}