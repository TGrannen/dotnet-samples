using KafkaFlow.Admin.Dashboard;
using KafkaFlow.Configuration;
using KafkaFlow.OpenTelemetry;
using KafkaFlowSample.Consumer.Configuration;
using KafkaFlowSample.Consumer.Middleware;
using KafkaSample.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.SetupSerilog();

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.Configure<TestingConfig>(builder.Configuration.GetSection("TestingConfig"));
builder.Services.AddOpenTelemetry().WithTracing(tracing => { tracing.AddSource(KafkaFlowInstrumentation.ActivitySourceName); });
builder.Services.AddScoped<ITestScopedService, TestScopedService>();

builder.SetupKafka(new Dictionary<string, Action<IConsumerMiddlewareConfigurationBuilder>>
{
    { "BatchTest", m => m.Add<BatchProcessingMiddleware>() },
    { "CDCTest", m => m.Add<CdcBatchProcessingMiddleware>() }
});
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseKafkaFlowDashboard();
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();