using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.LaunchDarkly.WebAPI.Controllers
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
        private readonly IFeatureService _featureService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFeatureService featureService)
        {
            _logger = logger;
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            bool enabled = await _featureService.IsEnabledAsync(Features.Feature1);
            var rng = new Random();
            return Enumerable.Range(1, 5 + (enabled ? 10 : 0)).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}