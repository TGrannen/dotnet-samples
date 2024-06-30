using AwsMessagingTest.Messages;

namespace AwsMessagingTest.BackgroundServices;

public class Worker(ILogger<Worker> logger, IMessagePublisher messagePublisher, IOptionsMonitor<TestingConfig> optionsMonitor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Service} has started", nameof(Worker));

        while (!stoppingToken.IsCancellationRequested)
        {
            await messagePublisher.PublishAsync(new ChatMessage
            {
                MessageDescription = DateTime.Now.ToString()
            }, stoppingToken);

            await Task.Delay(optionsMonitor.CurrentValue.RateOfChat, stoppingToken);
        }
    }
}