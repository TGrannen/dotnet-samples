using Scalar.AspNetCore;
using SilverbackSample.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSerilog();
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddTransient<TimeProvider>(_ => TimeProvider.System);

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:messaging")!;

builder.Services.AddSilverback()
    .WithConnectionToMessageBroker(options => options.AddKafka())
    .AddKafkaEndpoints(endpoints => endpoints
        .Configure(config => { config.BootstrapServers = connectionString; })
        .AddOutbound<SampleMessage>(endpoint => endpoint.ProduceTo("samples-basic"))
        .AddOutbound<SampleBatchMessage>(endpoint => endpoint.ProduceTo("samples-batch"))
    );

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();