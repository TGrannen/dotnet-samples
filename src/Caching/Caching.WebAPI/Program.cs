using Caching.WebAPI;
using Caching.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");
builder.AddRedisDistributedCache("redis");

builder.Services
    .AddFusionCache("product-cache")
    .AsKeyedServiceByCacheName()
    .WithDefaultEntryOptions(options =>
    {
        options.Duration = TimeSpan.FromMinutes(10);
        options.IsFailSafeEnabled = true;
        options.FailSafeMaxDuration = TimeSpan.FromHours(2);
        options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
        options.FactorySoftTimeout = TimeSpan.FromSeconds(1);
        options.FactoryHardTimeout = TimeSpan.FromSeconds(2);
        options.JitterMaxDuration = TimeSpan.FromSeconds(30);
    })
    .WithSystemTextJsonSerializer()
    .WithDistributedCache(serviceProvider => serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>());

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => { metrics.AddFusionCacheInstrumentation(); })
    .WithTracing(tracing => { tracing.AddFusionCacheInstrumentation(); });

var app = builder.Build();

// Ensure database exists and seed initial product data (development)
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new ProductEntity { Name = "Widget A", Sku = "SKU-001", UnitPrice = 19.99m, CreatedAt = DateTime.UtcNow },
            new ProductEntity { Name = "Gadget B", Sku = "SKU-002", UnitPrice = 49.99m, CreatedAt = DateTime.UtcNow },
            new ProductEntity { Name = "Gizmo C", Sku = "SKU-003", UnitPrice = 9.99m, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();