using AutoBogus;

namespace KafkaSample.ApiService;

public class ProducerService(IProducer<string, EventType1> producer, ILogger<ProducerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessKafkaMessage(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        try
        {
            var value = AutoFaker.Generate<EventType1>();
            logger.LogInformation("Sending message: {@Message}", value);
            await producer.ProduceAsync("ContinuousUpdates", new Message<string, EventType1>
            {
                Key = value.OrderNumber,
                Value = value,
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing Kafka message: {Message}", ex.Message);
        }
    }
}