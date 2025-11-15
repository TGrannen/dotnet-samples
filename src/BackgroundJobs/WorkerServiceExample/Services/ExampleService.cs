namespace WorkerServiceExample.Services;

public class ExampleService(ILogger<ExampleService> logger) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Service} up worker at: {Time}", nameof(ExampleService), DateTimeOffset.Now);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping {Service} worker at: {Time}", nameof(ExampleService), DateTimeOffset.Now);
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(2500, stoppingToken);
        }
    }
}