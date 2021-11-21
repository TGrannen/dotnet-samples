using App.Metrics;
using App.Metrics.Counter;

namespace Observability.WebAPI.Services
{
    public class MetricService
    {
        private readonly IMetrics _metrics;

        private static readonly CounterOptions CounterOptions = new()
        {
            Name = "Forecasts Calls",
            Context = "TestApi",
            MeasurementUnit = Unit.Requests
        };

        public MetricService(IMetrics metrics)
        {
            _metrics = metrics;
        }

        public void WeatherForecastIncrement()
        {
            _metrics.Measure.Counter.Increment(CounterOptions);
        }
    }
}