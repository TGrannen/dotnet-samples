using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Observability.WebAPI.Services;

namespace Observability.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly MetricService _metricService;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ActivityService _service;
        private readonly Random _random = new Random();

        public WeatherForecastController(MetricService metricService, ILogger<WeatherForecastController> logger, ActivityService service)
        {
            _metricService = metricService;
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogWarning("Getting Weather Forecasts");
            _metricService.WeatherForecastIncrement();

            var number = _random.Next(2, 10);
            _metricService.WeatherForecastReturned(number);

            await _service.Hello();
            await Task.Delay(400);
            await _service.Goodbye();

            return Enumerable.Range(1, number).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}