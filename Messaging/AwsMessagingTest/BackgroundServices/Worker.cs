using AwsMessagingTest.Messages;

namespace AwsMessagingTest.BackgroundServices;

public class Worker(ILogger<Worker> logger, IMessagePublisher bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Service} has started", nameof(Worker));

        while (!stoppingToken.IsCancellationRequested)
        {
            await bus.PublishAsync(new ChatMessage
            {
                MessageDescription = DateTime.Now.ToString()
            }, stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}