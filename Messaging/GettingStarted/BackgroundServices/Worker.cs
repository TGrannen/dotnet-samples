using Microsoft.Extensions.Hosting;

namespace GettingStarted;

public class Worker(ILogger<Worker> logger, IBus bus, IOptionsMonitor<TestingConfig> optionsMonitor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Service} has started", nameof(Worker));

        while (!stoppingToken.IsCancellationRequested)
        {
            await bus.Publish(new Hello
            {
                Value = DateTime.Now.ToString()
            }, stoppingToken);

            await Task.Delay(optionsMonitor.CurrentValue.RateOfMessage, stoppingToken);
        }
    }
}