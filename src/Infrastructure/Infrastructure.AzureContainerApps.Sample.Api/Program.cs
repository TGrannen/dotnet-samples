using Scalar.AspNetCore;
using Serilog;
using System.Reflection;

// Minimal Web API for the Azure Container Apps (Pulumi) sample pipeline.
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health")
    .WithSummary("Health check");

app.MapGet("/version", () =>
    {
        var sha = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        return Results.Ok(new
        {
            commit = string.IsNullOrWhiteSpace(sha) ? "dev" : sha,
        });
    })
    .WithName("Version")
    .WithSummary("Build commit SHA");

app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
