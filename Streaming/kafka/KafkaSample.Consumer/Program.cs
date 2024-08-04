using KafkaSample.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddKafkaConsumer<string, string>("messaging", consumerBuilder =>
{
    consumerBuilder.ConnectionString = builder.Configuration.GetConnectionString("messaging");
    consumerBuilder.Config.GroupId = "TEST";
});

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.Run();