using GoldenVitrin.NginxManager.Services.Nginx;
using GoldenVitrin.NginxManager.Services.Nginx.Business;
using GoldenVitrin.NginxManager.Services.Nginx.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddNginxManager(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/nginx/start", async (INginxManager manager, CancellationToken cancellationToken) =>
{
    var result = await manager.TryStartNginxAsync(cancellationToken);
    return string.IsNullOrEmpty(result) ? Results.Ok("nginx started...") : Results.InternalServerError(result);
});
app.MapGet("/nginx/quit", async (INginxManager manager, CancellationToken cancellationToken) =>
{
    var result = await manager.TryQuitNginxAsync(cancellationToken);
    return string.IsNullOrEmpty(result) ? Results.Ok("nginx quite...") : Results.InternalServerError(result);
});
app.MapGet("/nginx/reload", async (INginxManager manager, CancellationToken cancellationToken) =>
{
    var result = await manager.TryReloadNginxAsync(cancellationToken);
    return string.IsNullOrEmpty(result) ? Results.Ok("nginx reloaded...") : Results.InternalServerError(result);
});

app.MapGet("/nginx/domain/{domainName}", async (INginxManager manager, string domainName, CancellationToken CancellationToken) =>
    {
        var result = await manager.TryGetDomainSetting(domainName, CancellationToken).ConfigureAwait(false);
        return string.IsNullOrEmpty(result) ? Results.NotFound("Related domain settings not found!") : Results.Ok(result);
    });

app.MapPost("/nginx/domain", async ([FromBody] DomainSettings domainSettings, INginxManager manager, CancellationToken cancelationToken) =>
{
    var result = await manager.TryAddOrUpdateDomainAsync(domainSettings, cancelationToken).ConfigureAwait(false);
    return result.StartsWith("\\n")? Results.Ok(result):Results.InternalServerError(result);
});

app.MapPost("/domain", (AddDomainRequest request) =>
{
    return request;
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/", (INginxManager manager) => "Hi from nginx manager...");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class AddDomainRequest
{
    public string Name { get; set; }
    public bool HasSSl { get; set; }
}