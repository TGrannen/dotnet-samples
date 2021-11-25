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

        private readonly AppMetricsMetricService _appMetricsMetricService;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ActivityService _service;
        private readonly OtelMetricService _metricService;
        private readonly Random _random = new();

        public WeatherForecastController(AppMetricsMetricService appMetricsMetricService,
            ILogger<WeatherForecastController> logger,
            ActivityService service,
            OtelMetricService metricService)
        {
            _appMetricsMetricService = appMetricsMetricService;
            _logger = logger;
            _service = service;
            _metricService = metricService;
        }

        [HttpGet]
        [Route(nameof(GetWeatherForecastMetrics))]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastMetrics()
        {
            _logger.LogWarning("Getting Weather Forecasts - Metrics");
            _appMetricsMetricService.WeatherForecastIncrement();

            var number = _random.Next(2, 10);
            _appMetricsMetricService.WeatherForecastReturned(number);

            _metricService.RandomFruitAmount();

            await _metricService.RandomDelay();

            return Enumerable.Range(1, number).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet]
        [Route(nameof(GetWeatherForecastTrace))]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastTrace()
        {
            _logger.LogWarning("Getting Weather Forecasts - Trace");

            await _service.Hello();
            await Task.Delay(400);
            await _service.Goodbye();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}