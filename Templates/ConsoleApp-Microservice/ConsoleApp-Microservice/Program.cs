using ConsoleApp_Microservice;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ExampleBackgroundService>();

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.WebHost.UseHealthEndpoints();
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true);

var app = builder.Build();
app.UseSerilogRequestLogging();

await app.RunAsync();