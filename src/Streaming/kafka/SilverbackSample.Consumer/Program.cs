using SilverbackSample.Consumer;
using SilverbackSample.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSerilog();
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSilverback().WithConnectionToMessageBroker(options => options.AddKafka());

builder.Services.GenericSampleMessageConsumption()
    .AddScopedSubscriber<SampleMessageSubscriber>();

builder.Services.AddSampleMessageBatching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();