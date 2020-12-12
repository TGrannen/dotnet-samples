using BlazorServer.Data;
using BlazorServer.Store.Shared;
using System.Collections.Generic;

namespace BlazorServer.Store.Forecasts.Actions
{
    public class LoadForecastResultAction : ResultAction
    {
        public LoadForecastResultAction(string errorMessage) : base(errorMessage)
        {
            Forecasts = null!;
        }

        public LoadForecastResultAction(IEnumerable<WeatherForecast> forecastsResponse) : base(null)
        {
            Forecasts = forecastsResponse;
        }

        public IEnumerable<WeatherForecast> Forecasts { get; }
    }
}