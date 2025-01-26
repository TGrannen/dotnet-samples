using FeatureFlags.LaunchDarkly.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.LaunchDarkly.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LaunchDarklySDKController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly LaunchDarklyDirectService _featureService;
    private readonly IUserService _userService;

    public LaunchDarklySDKController(LaunchDarklyDirectService featureService, IUserService userService)
    {
        _featureService = featureService;
        _userService = userService;
    }

    [HttpGet]
    [Route("SampleOne")]
    public IEnumerable<WeatherForecast> Get()
    {
        var enabled = _featureService.IsSampleOneEnabled();
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
    public IActionResult GetContextual()
    {
        var user = _userService.GetUser();
        var enabled = _featureService.IsSampleTwoEnabled(user);

        return Ok(new
        {
            User = user,
            Enabled = enabled,
        });
    }

    [HttpGet]
    [Route("SampleJson")]
    public IActionResult JsonFeature()
    {
        var user = _userService.GetUser();
        var result = _featureService.JsonSample(user);
        return Ok(new
        {
            User = user,
            Result = result,
        });
    }

    [HttpGet]
    [Route("SampleOne/Contextual")]
    public IActionResult CustomContext()
    {
        var enabled = _featureService.IsSampleOneEnabledCustom();
        return Ok(new
        {
            Enabled = enabled,
        });
    }
}