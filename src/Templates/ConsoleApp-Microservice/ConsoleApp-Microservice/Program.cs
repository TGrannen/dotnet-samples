using ConsoleApp_Microservice;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

builder.Services.AddHostedService<ExampleBackgroundService>();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSerilogRequestLogging();

app.UseHealthChecks("/health");
app.MapOpenApi();

await app.RunAsync();