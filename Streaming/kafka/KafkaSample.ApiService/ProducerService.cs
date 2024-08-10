namespace KafkaSample.ApiService;

// public class ProducerService(IProducer<string, string> producer, ILogger<ProducerService> logger) : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             await ProcessKafkaMessage(stoppingToken);
//
//             await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//         }
//     }
//
//     private async Task ProcessKafkaMessage(CancellationToken stoppingToken)
//     {
//         try
//         {
//             var value = new
//             {
//                 Id = Guid.NewGuid()
//             };
//             logger.LogInformation("Sending message: {@Message}", value);
//             await producer.ProduceAsync("ContinuousUpdates", new Message<string, string>
//             {
//                 Key = "test",
//                 Value = JsonSerializer.Serialize(value),
//             }, stoppingToken);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error publishing Kafka message: {Message}", ex.Message);
//         }
//     }
// }