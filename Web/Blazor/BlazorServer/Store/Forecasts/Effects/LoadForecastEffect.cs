using BlazorServer.Data;
using BlazorServer.Store.Forecasts.Actions;
using Fluxor;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlazorServer.Store.Forecasts.Effects
{
    public class LoadForecastEffect : Effect<LoadForecastAction>
    {
        private readonly ILogger<LoadForecastEffect> _logger;
        private readonly WeatherForecastService _service;
        private static readonly Random Random = new Random();

        public LoadForecastEffect(ILogger<LoadForecastEffect> logger, WeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        protected override async Task HandleAsync(LoadForecastAction action, IDispatcher dispatcher)
        {
            try
            {
                _logger.LogInformation("Loading forecasts...");

                if (Random.Next(1, 3) == 1)
                {
                    await Task.Delay(1000);
                    throw new Exception("Random Error");
                }

                // Add a little extra latency for dramatic effect...
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                var forecastsResponse = await _service.GetForecastAsync(DateTime.Now);

                _logger.LogInformation("Forecasts loaded successfully!");
                dispatcher.Dispatch(new LoadForecastResultAction(forecastsResponse));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error loading forecasts, reason: {e.Message}");
                dispatcher.Dispatch(new LoadForecastResultAction(e.Message));
            }
        }
    }
}