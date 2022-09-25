using BlazorServer.Data;
using BlazorServer.Store.Shared;
using System.Collections.Generic;

namespace BlazorServer.Store.Forecasts;

public class ForecastState : RootState
{
    public ForecastState(bool isLoading, string? currentErrorMessage, IEnumerable<WeatherForecast>? currentForecasts)
        : base(isLoading, currentErrorMessage)
    {
        CurrentForecasts = currentForecasts;
    }

    public IEnumerable<WeatherForecast>? CurrentForecasts { get; }
}