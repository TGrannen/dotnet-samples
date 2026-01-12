using System;

namespace ConsoleApp_Microservice;

public class ExampleBackgroundService(ILogger<ExampleBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Service} has started", nameof(ExampleBackgroundService));
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("{Service} has Executed", nameof(ExampleBackgroundService));
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}