using AutoBogus;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaSample.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
{
    Url = builder.Configuration.GetConnectionString("schema")
});

builder.AddKafkaProducer<string, EventType1>("messaging", settings =>
{
    settings.SetValueSerializer(new JsonSerializer<EventType1>(schemaRegistry));
});
builder.AddKafkaProducer<string, EventType2>("messaging", settings =>
{
    settings.SetValueSerializer(new JsonSerializer<EventType2>(schemaRegistry));
});
builder.Services.AddHostedService<ProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/sendMessage", async (IProducer<string, EventType2> producer, ILogger<Program> logger) =>
{
    var value = AutoFaker.Generate<EventType2>();
    logger.LogInformation("Sending Test message: {@Message}", value);
    await producer.ProduceAsync("TestUpdate", new Message<string, EventType2>
    {
        Key = value.StoreNumber,
        Value = value,
    });

    return Results.Ok(value);
});

app.MapDefaultEndpoints();

app.Run();