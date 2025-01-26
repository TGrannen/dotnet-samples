using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Observability.WebAPI.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<AppMetricsMetricService>();
builder.Services.AddSingleton<OtelMetricService>();
builder.Services.AddSingleton<ActivityService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryTracing((telemetryBuilder) => telemetryBuilder
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Configuration.GetValue<string>("OpenTelemetry:Zipkin:ServiceName")))
    .AddSource(ActivityService.Name)
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddZipkinExporter());
builder.Services.Configure<ZipkinExporterOptions>(builder.Configuration.GetSection("OpenTelemetry:Zipkin"));

// Open Telemetry Metrics
builder.Services.AddOpenTelemetryMetrics((meterBuilder) => meterBuilder
    .AddMeter(OtelMetricService.MeterName)
    .AddPrometheusExporter()
);
builder.Services.Configure<PrometheusExporterOptions>(builder.Configuration.GetSection("OpenTelemetry:Prometheus"));

// AppMetrics
builder.Services.AddMetrics();
builder.WebHost.UseMetricsWebTracking()
    .UseMetrics(
        options =>
        {
            options.EndpointOptions = endpointsOptions =>
            {
                endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                endpointsOptions.EnvironmentInfoEndpointEnabled = false;
            };
        });
builder.WebHost.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
var app = builder.Build();

// Metrics scraping
app.UseOpenTelemetryPrometheusScrapingEndpoint();

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