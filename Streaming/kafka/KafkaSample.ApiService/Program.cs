using AutoBogus;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaSample.ApiService;
using KafkaSample.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddKafkaProducer<string, string>("messaging");
// builder.Services.AddHostedService<ProducerService>();

builder.Host
    .UseMassTransit((hostContext, x) =>
    {
        x.UsingInMemory();

        x.AddRider(r =>
        {
            r.AddKafkaComponents();

            var location = hostContext.Configuration.GetValue<int>("Warehouse:Location", 69);
            if (location == default)
                throw new ConfigurationException("The warehouse location is required and was not configured.");

            var warehouseTopicName = $"events.warehouse.{location}";

            r.AddProducer<string, WarehouseEvent>(warehouseTopicName, (context, cfg) =>
            {
                // Configure the AVRO serializer, with the schema registry client
                cfg.SetValueSerializer(new AvroSerializer<WarehouseEvent>(context.GetRequiredService<ISchemaRegistryClient>()));
            });

            r.AddProducer<string, ShipmentManifestReceived>("events.erp", (context, cfg) =>
            {
                // Configure the AVRO serializer, with the schema registry client
                cfg.SetValueSerializer(new AvroSerializer<ShipmentManifestReceived>(context.GetRequiredService<ISchemaRegistryClient>()));
            });

            r.UsingKafka((context, cfg) =>
            {
                cfg.ClientId = "WarehouseApi";

                cfg.Host(context);
            });
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/sendMessage", async (ITopicProducer<string, WarehouseEvent> producer, ILogger<Program> logger, CancellationToken token) =>
{
    var detail = AutoFaker.Generate<PickDetail>();
    logger.LogInformation("Sending Test message: {@Message}", detail);

    await producer.Produce($"{detail.OrderNumber}", new WarehouseEvent
    {
        SourceSystemId = detail.SourceSystemId,
        EventId = detail.EventId,
        Timestamp = detail.Timestamp?.ToString("O"),
        Event = new ProductPicked
        {
            OrderNumber = detail.OrderNumber,
            OrderLine = detail.OrderLine!.Value,
            Sku = detail.Sku,
            SerialNumber = detail.SerialNumber,
            LicensePlateNumber = detail.LicensePlateNumber
        }
    }, token);

    return Results.Ok(detail);
});

app.MapDefaultEndpoints();

app.Run();

public record PickDetail
{
    public required string? SourceSystemId { get; init; }
    public required string? EventId { get; init; }
    public DateTimeOffset? Timestamp { get; init; }
    public required string? OrderNumber { get; init; }
    public required long? OrderLine { get; init; }
    public required string? Sku { get; init; }
    public required string? SerialNumber { get; init; }
    public required string? LicensePlateNumber { get; init; }
}