using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_Microservice
{
    public class ExampleBackgroundService : BackgroundService
    {
        private readonly ILogger<ExampleBackgroundService> _logger;

        public ExampleBackgroundService(ILogger<ExampleBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} has started", nameof(ExampleBackgroundService));
            return Task.CompletedTask;
        }
    }
}