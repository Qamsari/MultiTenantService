using Server.Services.TenantAccessor;
using Server.Services.DataService;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTenantAccessor();
builder.Services.AddDbService(builder.Configuration);
builder.Services.AddRazorPages();

var app = builder.Build();
//app.Use(async (HttpContext x, RequestDelegate y) => {
//    var host = x.Request.Host;
//    if (!x.Response.HasStarted) {
//        await x.Response.WriteAsync(host.Value??"??");
//    }
//});
app.MapRazorPages();
//app.MapGet("/", () => "Hello World!");
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ISeedData>().SeedData();
}
app.Run();
