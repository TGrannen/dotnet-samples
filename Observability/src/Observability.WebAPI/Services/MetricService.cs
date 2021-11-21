using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Histogram;

namespace Observability.WebAPI.Services
{
    public class MetricService
    {
        private readonly IMetrics _metrics;

        private static readonly CounterOptions CounterOptions = new()
        {
            Name = "forecast_requests",
            Context = "TestApi",
            MeasurementUnit = Unit.Requests
        };

        private static readonly HistogramOptions HistogramOptions = new()
        {
            Name = "forecast_calls",
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

        public void WeatherForecastReturned(int value)
        {
            _metrics.Measure.Histogram.Update(HistogramOptions, value);
        }
    }
}