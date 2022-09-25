using Serilog;
using Configuration.Web.Extensions;
using Configuration.Web.Models;
using Configuration.Web.Providers.CustomProvider;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Configuration.AddEntityConfiguration();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Configuration.Web", Version = "v1" }); });

builder.Services.Configure<Settings1>(builder.Configuration.GetSection("Settings1"));
builder.Services.AddScoped<ISettings1>(provider =>
{
    var config = provider.GetService<IConfiguration>();
    var shouldUseInitialValue = config.GetValue<bool>("InjectedInterfaceShouldUsingInitialValue");
    return shouldUseInitialValue
        ? provider.GetService<IOptions<Settings1>>()?.Value
        : provider.GetService<IOptionsSnapshot<Settings1>>()?.Value;
});

builder.Services.Configure<ISettings2, Settings2>(builder.Configuration.GetSection("Settings2"));
builder.Services.Configure<Settings3>(builder.Configuration.GetSection("Settings3"));
builder.Services.AddScoped<ISettings3>(provider => provider.GetService<IOptionsSnapshot<Settings3>>()?.Value);

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