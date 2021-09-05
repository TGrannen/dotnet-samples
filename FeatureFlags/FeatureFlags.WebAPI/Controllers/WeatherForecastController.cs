using System;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.WebAPI.Feature;
using FeatureFlags.WebAPI.Feature.Filters;
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

        private readonly IFeatureManager _featureManager;
        private readonly IFeatureService _featureService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IFeatureManager featureManager, IFeatureService featureService,
            ILogger<WeatherForecastController> logger)
        {
            _featureManager = featureManager;
            _featureService = featureService;
            _logger = logger;
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

        [HttpGet]
        [Route("testContextual")]
        public async Task<IActionResult> GetTestContextual()
        {
            var rng = new Random();
            var context = new CustomContextualFilter.FilterContext
            {
                RandomNumber = rng.Next(-20, 55)
            };
            _logger.LogInformation("Context: {@Context}", context);
            if (await _featureService.IsNotEnabledAsync(Features.AllowForMinNumber, context))
            {
                return BadRequest();
            }

            return Ok(context);
        }

        private async Task<IActionResult> GetForecasts()
        {
            if (!await _featureManager.IsEnabledAsync("ShowWeather"))
            {
                return Unauthorized();
            }

            if (await _featureService.IsNotEnabledAsync(Features.AllowedForEndpoint))
            {
                return BadRequest();
            }

            var shouldBeCold = await _featureManager.IsEnabledAsync("ShouldBeSuperCold");
            var shouldHaveOnlyOne = await _featureService.IsEnabledAsync(Features.ShouldHaveOnlyOne);

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