using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Models;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.LaunchDarkly.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Names =
        {
            "James", "Sarah", "David", "Mia",
        };

        private static readonly string[] Id =
        {
            "1321", "5412", "76534", "3424",
        };

        private readonly IBoolFeatureService _featureService;
        private readonly IJsonFeatureService _jsonFeatureService;

        public WeatherForecastController(IBoolFeatureService featureService, IJsonFeatureService jsonFeatureService)
        {
            _featureService = featureService;
            _jsonFeatureService = jsonFeatureService;
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
            var (id, name) = GetUser();
            bool enabled = await _featureService.IsEnabledAsync(Features.Feature2, new UserWithNameContext
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

        [HttpGet]
        [Route("JsonFeature")]
        public async Task<IActionResult> JsonFeature()
        {
            var (id, name) = GetUser();
            var result = await _jsonFeatureService.GetFeatureConfigurationAsync<Feature3>(Features.Feature3, new UserWithNameContext
            {
                Id = id,
                Name = name
            });

            return Ok(new
            {
                Id = id,
                Name = name,
                Result = result,
            });
        }

        private (string id, string name) GetUser()
        {
            var rng = new Random();
            var index = rng.Next(0, 3);
            var name = Names[index];
            var id = Id[index];
            return (id, name);
        }
    }
}