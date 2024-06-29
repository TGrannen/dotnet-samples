using System;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace GettingStarted;

public class Worker(ILogger<Worker> logger, IBus bus) : BackgroundService
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

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}