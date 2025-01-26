using HealthChecks.UI.Client;
using HealthEndpoints.Web.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHealthChecks()
    .AddCheck<RandomHealthCheck>("Random", null, new[] { "Dummy" })
    .AddCheck<RandomHealthCheck>("Random2", null, new[] { "Dummy" })
    .AddCheck<RandomHealthCheck>("Random3", null, new[] { "Dummy" })
    .AddCheck("Inline", () => HealthCheckResult.Degraded("I'm not sure what's going on here"), new[] { "sql" })
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

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecksUI();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecks("/health/sql", new HealthCheckOptions
    {
        Predicate = c => c.Tags.Contains("sql"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

await app.RunAsync();