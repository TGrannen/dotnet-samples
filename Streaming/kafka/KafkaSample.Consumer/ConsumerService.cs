using Confluent.Kafka;

namespace KafkaSample.Consumer;

public class ConsumerService(IConsumer<string, string> consumer, ILogger<ConsumerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(new[] { "ContinuousUpdates", "TestUpdate" });

        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessKafkaMessage(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        consumer.Close();
    }

    private void ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        try
        {
            var consumeResult = consumer.Consume(stoppingToken);

            var message = consumeResult.Message.Value;

            logger.LogInformation("Received {Topic}: {Message}", consumeResult.Topic, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Kafka message: {Message}", ex.Message);
        }
    }
}