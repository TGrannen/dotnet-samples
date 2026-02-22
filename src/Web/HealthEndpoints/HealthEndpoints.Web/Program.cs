using HealthChecks.UI.Client;
using HealthEndpoints.Web.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHealthChecks()
    .AddCheck<RandomHealthCheck>("Random", null, ["Dummy"])
    .AddCheck<RandomHealthCheck>("Random2", null, ["Dummy"])
    .AddCheck<RandomHealthCheck>("Random3", null, ["Dummy"])
    .AddCheck("Inline", () => HealthCheckResult.Degraded("I'm not sure what's going on here"), ["sql"])
    ;

builder.Services
    .AddHealthChecksUI(setup => setup.DisableDatabaseMigrations())
    .AddInMemoryStorage();

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// Only redirect HTTP->HTTPS when the app is actually listening on HTTPS (e.g. avoids failing smoke tests that use http only)
var urls = builder.Configuration["ASPNETCORE_URLS"] ?? builder.Configuration["urls"] ?? "";
if (urls.Contains("https://", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}
app.UseRouting();

app.MapHealthChecksUI();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/sql", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("sql"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();