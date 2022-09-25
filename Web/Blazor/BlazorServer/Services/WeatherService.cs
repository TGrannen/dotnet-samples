using BlazorServer.Store.Forecasts.Actions;
using Fluxor;
using Microsoft.Extensions.Logging;

namespace BlazorServer.Services;

public class WeatherService
{
    private readonly ILogger<WeatherService> _logger;
    private readonly IDispatcher _dispatcher;

    public WeatherService(ILogger<WeatherService> logger, IDispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public void LoadForecasts()
    {
        _logger.LogInformation("Issuing action to load forecasts...");
        _dispatcher.Dispatch(new LoadForecastAction());
    }
}