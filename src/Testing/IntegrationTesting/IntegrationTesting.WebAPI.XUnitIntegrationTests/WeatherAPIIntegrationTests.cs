using IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

namespace IntegrationTesting.WebAPI.XUnitIntegrationTests;

public class WeatherAPIIntegrationTests : IClassFixture<AppFactory<Program>>
{
    private readonly IWeatherAPI _api;

    public WeatherAPIIntegrationTests(AppFactory<Program> factory)
    {
        _api = factory.CreateRefitClient<IWeatherAPI>();
    }

    [Fact]
    public async Task GetForecasts_ShouldReturn5Forecasts()
    {
        var forecasts = await _api.GetForecasts();
        forecasts.Count.ShouldBe(5);
        foreach (var x in forecasts)
        {
            x.ShouldSatisfyAllConditions(
                () => x.Date.ShouldBeGreaterThan(DateTime.MinValue),
                () => x.Summary.ShouldNotBeNull()
            );
        }
    }
}

internal interface IWeatherAPI
{
    [Get("/WeatherForecast")]
    Task<List<WeatherForecastDto>> GetForecasts();
}

public class WeatherForecastDto
{
    public DateTime Date { get; init; }

    public int TemperatureC { get; init; }

    public int TemperatureF { get; init; }

    public string? Summary { get; init; }
}