using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.LaunchDarkly.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly LibraryService _featureService;
        private readonly IUserService _userService;

        public LibraryController(LibraryService featureService, IUserService userService)
        {
            _featureService = featureService;
            _userService = userService;
        }

        [HttpGet]
        [Route("SampleOne")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var enabled = await _featureService.IsSampleOneEnabled();
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
        [Route("SampleTwo")]
        public async Task<IActionResult> GetContextual()
        {
            var user = _userService.GetUser();
            var enabled = await _featureService.IsSampleTwoEnabled(user);

            return Ok(new
            {
                User = user,
                Enabled = enabled,
            });
        }

        [HttpGet]
        [Route("SampleJson")]
        public async Task<IActionResult> JsonFeature()
        {
            var user = _userService.GetUser();
            var result = await _featureService.JsonSample(user);
            return Ok(new
            {
                User = user,
                Result = result,
            });
        }

        [HttpGet]
        [Route("SampleOne/Contextual")]
        public async Task<IActionResult> CustomContext()
        {
            var enabled = await _featureService.IsSampleOneEnabledCustom();
            return Ok(new
            {
                Enabled = enabled,
            });
        }
    }
}