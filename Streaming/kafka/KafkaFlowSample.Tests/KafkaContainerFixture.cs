using Testcontainers.Kafka;

namespace KafkaFlowSample.Tests;

public class KafkaContainerFixture : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
        .WithImage("confluentinc/cp-kafka:latest")
        .WithPortBinding(9092)
        .Build();

    public IConsumer<Ignore, string>? Consumer { get; private set; }
    public IProducer<Null, string>? Producer { get; private set; }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();

        var producerConfig = new ProducerConfig { BootstrapServers = _kafkaContainer.GetBootstrapAddress() };
        Producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        Consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
            {
                BootstrapServers = _kafkaContainer.GetBootstrapAddress(),
                GroupId = "demo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            })
            .Build();
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.DisposeAsync();
        Consumer!.Dispose();
        Producer!.Dispose();
    }
}