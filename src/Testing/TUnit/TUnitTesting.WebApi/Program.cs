using Microsoft.Extensions.Http.Resilience;
using Refit;
using Scalar.AspNetCore;
using Serilog;
using TUnitTesting.WebApi.Clients;
using TUnitTesting.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);

// Refit paths start with '/'; a BaseAddress ending with '/' yields a '//' in the merged URL.

builder.Services.AddRefitClient<IDownstreamCatalogApi>()
    .ConfigureHttpClient(client =>
    {
        var downstreamCatalogBaseUrl = (builder.Configuration["DownstreamCatalog:BaseUrl"] ?? "http://127.0.0.1:1").TrimEnd('/');
        client.BaseAddress = new Uri(downstreamCatalogBaseUrl);
    })
    .AddStandardResilienceHandler(options =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            // Looser bounds for tests that advance FakeTimeProvider (Polly uses TimeProvider for delays).
            // Circuit breaker sampling must be at least twice the attempt timeout (options validation).
            options.AttemptTimeout.Timeout = TimeSpan.FromMinutes(2);
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(5);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10);
        }
    });

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

if (!builder.Environment.IsEnvironment("Testing"))
{
    app.UseSerilogRequestLogging();
}
else
{
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace TUnitTesting.WebApi
{
    public partial class Program
    {
    }
}