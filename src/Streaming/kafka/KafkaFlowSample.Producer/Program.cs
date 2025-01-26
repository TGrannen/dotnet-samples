using KafkaFlow.Configuration;
using KafkaFlow.OpenTelemetry;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using KafkaFlowSample.MessageContracts;
using KafkaFlowSample.Producer;
using KafkaSample.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.SetupSerilog();
builder.Services.AddOpenApi();

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOpenTelemetry().WithTracing(tracing => { tracing.AddSource(KafkaFlowInstrumentation.ActivitySourceName); });
const string topicName = "sample-topic";
const string delayTopic = "delay-topic";
const string batchTopic = "batch-topic";
const string cdcTopic = "cdc-topic";

const string producerName = "say-hello";
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:messaging");

builder.Services.AddKafkaFlowHostedService(
    kafka => kafka
        .UseMicrosoftLog()
        .AddOpenTelemetryInstrumentation()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { connectionString })
                .CreateTopicIfNotExists(topicName, 10, 1)
                .CreateTopicIfNotExists(delayTopic, 10, 1)
                .CreateTopicIfNotExists(batchTopic, 10, 1)
                .CreateTopicIfNotExists(cdcTopic, 10, 1)
                .OnStarted(resolver => { resolver.Resolve<ILogger<Program>>().LogInformation("Started Kafka Cluster"); })
                .OnStopping(resolver => { resolver.Resolve<ILogger<Program>>().LogInformation("Stopping Kafka Cluster"); })
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )
        )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost("/produce", async (IProducerAccessor producerAccessor, ILogger<Program> logger) =>
{
    var producer = producerAccessor.GetProducer(producerName);

    var messageValue = new HelloMessage($"Hello! {DateTime.Now}");
    await producer.ProduceAsync(Guid.NewGuid().ToString(), messageValue);
    logger.LogInformation("Sent: {@Message}", messageValue);
    return messageValue;
});

app.MapPost("/produce/delay",
    async ([FromServices] IProducerAccessor producerAccessor, [FromServices] ILogger<Program> logger, TimeSpan? delay = null) =>
    {
        var producer = producerAccessor.GetProducer(producerName);
        var messageValue = new DelayMessage(delay ?? TimeSpan.FromSeconds(15));
        await producer.ProduceAsync(delayTopic, Guid.NewGuid().ToString(), messageValue);
        logger.LogInformation("Sent: {@Message}", messageValue);
        return messageValue;
    });

app.MapPost("/produce/batch",
    async ([FromServices] IProducerAccessor producerAccessor, [FromServices] ILogger<Program> logger, int count = 10) =>
    {
        var producer = producerAccessor.GetProducer(producerName);
        var messages = Enumerable
            .Range(0, count)
            .Select(
                _ => new BatchProduceItem(
                    batchTopic,
                    Guid.NewGuid().ToString(),
                    new SampleBatchMessage { Text = Guid.NewGuid().ToString() },
                    null))
            .ToList();
        var result = await producer.BatchProduceAsync(messages);
        logger.LogInformation("Sent {Count} messages", count);
        return result;
    });

app.MapPost("/produce/bad-data",
    async (bool throwException, IProducerAccessor producerAccessor, TimeProvider timeProvider, ILogger<Program> logger) =>
    {
        var producer = producerAccessor.GetProducer(producerName);
        var messageValue = new BadDataMessage(throwException, timeProvider.GetLocalNow().DateTime);
        await producer.ProduceAsync(Guid.NewGuid().ToString(), messageValue);
        logger.LogInformation("Sent: {@Message}", messageValue);
        return messageValue;
    });

app.MapPost("/produce/cdc-batch",
    async ([FromServices] IProducerAccessor producerAccessor, [FromServices] ILogger<Program> logger, int count = 10) =>
    {
        var producer = producerAccessor.GetProducer(producerName);
        var messages = Enumerable
            .Range(0, count)
            .Select(
                _ => new BatchProduceItem(
                    cdcTopic,
                    Guid.NewGuid().ToString(),
                    CDCGenerator.Generate(),
                    null))
            .ToList();
        var result = await producer.BatchProduceAsync(messages);
        logger.LogInformation("Sent {Count} CDC messages", count);
        return result;
    });

app.Run();

namespace KafkaFlowSample.Producer
{
    internal class SampleBatchMessage
    {
        public string Text { get; set; }
    }
}