using Serilog;
using Configuration.Web.Extensions;
using Configuration.Web.Models;
using Configuration.Web.Providers.CustomProvider;
using Microsoft.OpenApi.Models;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Configuration.AddCustomConfiguration();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Configuration.Web", Version = "v1" }); });

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program), ServiceLifetime.Transient);
builder.Services.ConfigureValidated<Settings1>(builder.Configuration.GetSection("Settings1"));
builder.Services.ConfigureValidated<Settings2>(builder.Configuration.GetSection("Settings2"));
builder.Services.ConfigureValidated<Settings3>(builder.Configuration.GetSection("Settings3"));

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Configuration.Web v1"));

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

try
{
    Log.Information("Starting up");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}