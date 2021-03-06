using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceExample.Services
{
    public class HttpPollExample : BackgroundService
    {
        private readonly ILogger<HttpPollExample> _logger;
        private readonly IHttpClientFactory _factory;

        public HttpPollExample(ILogger<HttpPollExample> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var success = await IsGoogleUp();
                    _logger.Log(success
                            ? LogLevel.Information
                            : LogLevel.Error,
                        "Google is {Status}",
                        success ? "Online" : "Offline");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Major Exception");
                }
                finally
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private async Task<bool> IsGoogleUp()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("http://google.com");
            bool success = response.IsSuccessStatusCode;
            return success;
        }
    }
}