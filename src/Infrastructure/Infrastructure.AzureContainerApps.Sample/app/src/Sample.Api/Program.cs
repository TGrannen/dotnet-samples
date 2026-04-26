using Scalar.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

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

