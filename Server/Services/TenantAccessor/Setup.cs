namespace Server.Services.TenantAccessor;

public static class Setup
{
    public static IServiceCollection AddTenantAccessor(this IServiceCollection serviceDescriptors)
        =>serviceDescriptors.AddScoped<ITenantAccessor,TenantAccessorService>();
}