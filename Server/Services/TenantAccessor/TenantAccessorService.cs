namespace Server.Services.TenantAccessor
{
     class TenantAccessorService(IHttpContextAccessor httpContextAccessor) : ITenantAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string? Tenant => _httpContextAccessor.HttpContext?.Request.Host.Value;
    }
}
