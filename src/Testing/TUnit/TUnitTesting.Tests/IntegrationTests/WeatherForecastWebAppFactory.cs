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
}