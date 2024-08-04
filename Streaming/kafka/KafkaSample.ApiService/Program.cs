using KafkaSample.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddKafkaProducer<string, string>("messaging");
builder.Services.AddHostedService<ProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/sendMessage", async (IProducer<string, string> producer, ILogger<Program> logger) =>
{
    var value = new
    {
        Id = Guid.NewGuid()
    };
    logger.LogInformation("Sending Test message: {@Message}", value);
    await producer.ProduceAsync("TestUpdate", new Message<string, string>
    {
        Key = "test",
        Value = JsonSerializer.Serialize(value),
    });

    return Results.Ok(value);
});

app.MapDefaultEndpoints();

app.Run();