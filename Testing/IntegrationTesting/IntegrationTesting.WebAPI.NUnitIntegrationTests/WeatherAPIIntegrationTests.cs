namespace IntegrationTesting.WebAPI.NUnitIntegrationTests;

public class WeatherAPIIntegrationTests
{
    private IWeatherAPI _api;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _api = new WebApplicationFactory<Program>().CreateRefitClient<Program, IWeatherAPI>();
    }

    [Test]
    public async Task GetForecasts_ShouldReturn5Forecasts()
    {
        var forecasts = await _api.GetForecasts();
        forecasts.Should().HaveCount(5);
        forecasts.Should().AllSatisfy(x =>
        {
            x.Date.Should().BeAfter(DateTime.MinValue);
            x.Summary.Should().NotBeNull();
        });
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

    public string Summary { get; init; }
}