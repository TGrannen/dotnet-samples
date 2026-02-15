using Caching.WebAPI;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    var otlpEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
    {
        configuration.WriteTo.OpenTelemetry(options =>
        {
            options.Endpoint = otlpEndpoint;
            options.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = context.Configuration["OTEL_SERVICE_NAME"] ?? context.HostingEnvironment.ApplicationName ?? "Caching.WebAPI"
            };
        });
    }
}, writeToProviders: false);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSwaggerGen();

builder.AddRedisDistributedCache("cache");

builder.Services
    .AddFusionCache()
    .WithDefaultEntryOptions(options =>
    {
        options.Duration = TimeSpan.FromMinutes(10);
        options.IsFailSafeEnabled = true;
        options.FailSafeMaxDuration = TimeSpan.FromHours(2);
        options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
        options.FactorySoftTimeout = TimeSpan.FromSeconds(1);
        options.FactoryHardTimeout = TimeSpan.FromSeconds(2);
    })
    .WithSystemTextJsonSerializer()
    .WithDistributedCache(serviceProvider => serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>());

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => { metrics.AddFusionCacheInstrumentation(); })
    .WithTracing(tracing =>
    {
        tracing
            .AddFusionCacheInstrumentation()
            .AddSource(Tracing.SourceName);
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();