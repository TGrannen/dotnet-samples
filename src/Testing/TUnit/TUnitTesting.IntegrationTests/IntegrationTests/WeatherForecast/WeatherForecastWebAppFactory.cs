using Microsoft.AspNetCore.Hosting;

namespace TUnitTesting.IntegrationTests.IntegrationTests.WeatherForecast;

public class WeatherForecastWebAppFactory : TestWebApplicationFactory<WebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
