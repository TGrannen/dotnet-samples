using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace Observability.WebAPI.Services
{
    // ReSharper disable once IdentifierTypo
    public class OtelMetricService
    {
        public const string MeterName = "Observibility.WebAPI.ManualMetrics";
        private static readonly Meter MyMeter = new(MeterName);
        private static readonly Counter<long> MyFruitCounter = MyMeter.CreateCounter<long>("MyFruitCounter");
        private static readonly Histogram<long> MyHistogram = MyMeter.CreateHistogram<long>("RequestHistorgram");
        private static readonly Counter<long> ForecastRequestsCounter = MyMeter.CreateCounter<long>("forecast_requests", "Requests", "Number of weather forecast requests");
        private static readonly Histogram<int> ForecastCallsHistogram = MyMeter.CreateHistogram<int>("forecast_calls", "Requests", "Number of forecasts returned per call");
        private readonly Random _random = new();

        public void RandomFruitAmount()
        {
            MyFruitCounter.Add(_random.Next(1, 14), KeyValuePair.Create<string, object?>("name", nameof(RandomFruitAmount)));
        }

        public async Task RandomDelay()
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(_random.Next(100, 600));

            // Measure the duration in ms of requests and includes the host in the tags
            MyHistogram.Record(stopwatch.ElapsedMilliseconds, KeyValuePair.Create<string, object?>("Host", "github.com"));
        }

        /// <summary>
        /// Records that a weather forecast request was made (replaces App.Metrics forecast_requests counter).
        /// </summary>
        public void WeatherForecastRequestIncrement()
        {
            ForecastRequestsCounter.Add(1, KeyValuePair.Create<string, object?>("Context", "TestApi"));
        }

        /// <summary>
        /// Records the number of forecast items returned (replaces App.Metrics forecast_calls histogram).
        /// </summary>
        public void WeatherForecastReturned(int count)
        {
            ForecastCallsHistogram.Record(count, KeyValuePair.Create<string, object?>("Context", "TestApi"));
        }
    }
}