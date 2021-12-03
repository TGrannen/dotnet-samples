using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.Library;
using FeatureFlags.LaunchDarkly.Library.Context;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Models;
using FeatureFlags.LaunchDarkly.WebAPI.Services;
using LaunchDarkly.Sdk;
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
        
        private readonly IFeatureService _featureService;
        private readonly IJsonFeatureService _jsonFeatureService;
        private readonly IUserService _userService;

        public WeatherForecastController(IFeatureService featureService, IJsonFeatureService jsonFeatureService, IUserService userService)
        {
            _featureService = featureService;
            _jsonFeatureService = jsonFeatureService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var enabled = await _featureService.IsEnabledAsync("demo-sample-feature");
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
            var user = _userService.GetUser();
            var enabled = await _featureService.IsEnabledAsync("demo-sample-feature-2", new FeatureContext
            {
                Key = user.Id,
                Name = new ContextAttribute<string>(user.Name)
            });

            return Ok(new
            {
                User = user,
                Enabled = enabled,
            });
        }

        [HttpGet]
        [Route("JsonFeature")]
        public async Task<IActionResult> JsonFeature()
        {
            var user = _userService.GetUser();
            var result = await _jsonFeatureService.GetJsonConfiguration<Feature3Dto>("demo-json-feature",
                new FeatureContext
                {
                    Key = user.Id,
                    Name = new ContextAttribute<string>(user.Name)
                });
            return Ok(new
            {
                User = user,
                Result = result,
            });
        }


        [HttpGet]
        [Route("CustomContext")]
        public async Task<IActionResult> CustomContext()
        {
            var enabled = await _featureService.IsEnabledAsync("demo-sample-feature", new FeatureContext
            {
                Key = "TEST",
                CustomContextAttributes = new List<CustomContextAttribute<string>>
                {
                    new("My Data Stuff", "My fancy value")
                }
            });
            return Ok(new
            {
                Enabled = enabled,
            });
        }
    }
}