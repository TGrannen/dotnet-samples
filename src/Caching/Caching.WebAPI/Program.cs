using Caching.WebAPI;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();