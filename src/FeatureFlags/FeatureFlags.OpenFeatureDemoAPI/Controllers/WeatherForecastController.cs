using Microsoft.AspNetCore.Mvc;
using OpenFeature.Constant;

namespace FeatureFlags.OpenFeatureDemoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    IFeatureClient featureClient,
    [FromKeyedServices("flag-d")] IFeatureClient flagdClient,
    [FromKeyedServices("configuration")] IFeatureClient configClient,
    ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    public async Task<IActionResult> GetTest1()
    {
        return await GetForecasts();
    }

    [HttpGet]
    [Route("testContextual")]
    public async Task<IActionResult> GetTestContextual()
    {
        var rng = new Random();

        var value = rng.Next(-20, 55);
        var builder = EvaluationContext.Builder();
        builder.Set("RandomNumber", value);

        logger.LogInformation("RandomNumber: {@Context}", value);

        if (await featureClient.GetBooleanValueAsync("AllowForMinNumber", false, builder.Build()))
        {
            return BadRequest();
        }

        return Ok(builder.Build().AsDictionary());
    }

    [HttpGet]
    [Route("MultipleProviders")]
    public async Task<IActionResult> GetMultipleProviders()
    {
        var rng = new Random();

        var value = rng.Next(-20, 55);
        var ctx = EvaluationContext.Builder().Set("RandomNumber", value).Build();

        logger.LogInformation("RandomNumber: {@Context}", value);

        var flagd = await flagdClient.GetBooleanValueAsync("AllowForMinNumber", false, ctx);
        var config = await configClient.GetBooleanValueAsync("AllowForMinNumber", false, ctx);

        return Ok(new
        {
            value,
            flagd,
            config
        });
    }

    private async Task<IActionResult> GetForecasts()
    {
        if (featureClient.ProviderStatus != ProviderStatus.Ready)
        {
            throw new Exception("Feature status was " + featureClient.ProviderStatus);
        }

        if (!await featureClient.GetBooleanValueAsync("ShowWeather", false))
        {
            return Unauthorized();
        }

        var shouldBeCold = await featureClient.GetBooleanValueAsync("ShouldBeSuperCold", false);
        var shouldHaveOnlyOne = await featureClient.GetBooleanValueAsync("ShouldHaveOnlyOne", false);

        var rng = new Random();
        var weatherForecasts =
            Enumerable.Range(1, shouldHaveOnlyOne ? 1 : 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55) - (shouldBeCold ? 900 : 0),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        return Ok(weatherForecasts);
    }

    private class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}