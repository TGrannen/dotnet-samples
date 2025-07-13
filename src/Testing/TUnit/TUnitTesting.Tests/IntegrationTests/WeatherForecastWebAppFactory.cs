using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TUnitTesting.Tests.IntegrationTests;

public class WeatherForecastWebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public Task InitializeAsync()
    {
        // Grab a reference to the server
        // This forces it to initialize.
        // By doing it within this method, it's thread safe.
        // And avoids multiple initialisations from different tests if parallelisation is switched on
        _ = Server;
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        base.ConfigureWebHost(builder);
    }
}