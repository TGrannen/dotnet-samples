using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace FeatureFlags.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFeatureManager _featureManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        [HttpGet]
        [Route("test1")]
        public async Task<IActionResult> GetTest1()
        {
            return await GetForecasts();
        }

        [HttpGet]
        [Route("test2")]
        public async Task<IActionResult> GetTest2()
        {
            return await GetForecasts();
        }

        private async Task<IActionResult> GetForecasts()
        {
            if (!await _featureManager.IsEnabledAsync("AllowedForEndpoint"))
            {
                return BadRequest();
            }
            
            if (!await _featureManager.IsEnabledAsync("ShowWeather"))
            {
                return Unauthorized();
            }

            var shouldBeCold = await _featureManager.IsEnabledAsync("ShouldBeSuperCold");
            var shouldHaveOnlyOne = await _featureManager.IsEnabledAsync("ShouldHaveOnlyOne");

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
    }
}