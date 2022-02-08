using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Cocona;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

var config = InitConfig();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var builder = CoconaApp.CreateBuilder();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders().AddSerilog());

var app = builder.Build();

app.AddCommand("test", (string name, int count, [FromService] ILogger<Program> logger) =>
{
    if (count > 1)
    {
        var continueWithMultiple = Prompt.GetYesNo($"Are you sure that you want to run this {count} times?", true, ConsoleColor.Yellow);
        if (!continueWithMultiple)
        {
            logger.LogInformation("Cancelling {Name} command", "test");
            return 1;
        }
    }

    for (var i = 0; i < count; i++)
    {
        logger.LogInformation("Hello {Name}", name ?? "world");
    }

    return 0;
});

app.Run();

static IConfigurationRoot InitConfig()
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env}.json", true, true)
        .AddEnvironmentVariables();

    return builder.Build();
}