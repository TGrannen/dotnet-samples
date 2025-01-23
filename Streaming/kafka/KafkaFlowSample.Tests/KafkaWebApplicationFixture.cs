using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.Kafka;

namespace KafkaFlowSample.Tests;

[CollectionDefinition(nameof(KafkaWebApplicationFixture))]
public class KafkaWebApplicationFixtureCollection : ICollectionFixture<KafkaWebApplicationFixture>
{
}

public class KafkaWebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
        .WithImage("confluentinc/cp-kafka:latest")
        .Build();

    public IConsumer<Ignore, string>? Consumer { get; private set; }
    public FakeTimeProvider TimeProvider = new(DateTimeOffset.Parse("4/12/2021"));
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

        Environment.SetEnvironmentVariable("ConnectionStrings__messaging", _kafkaContainer.GetBootstrapAddress());
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.DisposeAsync();
        Consumer!.Dispose();
        Producer!.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(TimeProvider));
            services.AddSingleton<TimeProvider>(TimeProvider);
        });
    }
}