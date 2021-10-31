using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using Microsoft.AspNetCore.Mvc;

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

        private static readonly string[] Names = new[]
        {
            "James", "Sarah", "David", "Mia",
        }; 
        
        private static readonly string[] Id = new[]
        {
            "1321", "5412", "76534", "3424",
        };

        private readonly IBoolFeatureService _featureService;

        public WeatherForecastController(IBoolFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            bool enabled = await _featureService.IsEnabledAsync(Features.Feature1);
            var rng = new Random();
            return Enumerable.Range(1, enabled ? 10 : 0).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }


        [HttpGet]
        [Route("Contextual")]
        public async Task<IActionResult> GetContextual()
        {
            var rng = new Random();
            var index = rng.Next(0, 3);
            var name = Names[index];
            var id = Id[index];
            bool enabled = await _featureService.IsEnabledAsync(Features.Feature2, new Feature2Context
            {
                Id = id,
                Name = name
            });

            return Ok(new
            {
                Id = id,
                Name = name,
                Enabled = enabled,
            });
        }
    }
}