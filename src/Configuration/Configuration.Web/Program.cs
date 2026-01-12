using Serilog;
using Configuration.Web.Extensions;
using Configuration.Web.Models;
using Configuration.Web.Providers;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var systemsManagerEnabled = builder.Configuration.GetValue("AWS:SystemsManager:Enabled", false);
if (systemsManagerEnabled)
{
    var reloadAfter = builder.Configuration.GetValue("AWS:SystemsManager:ReloadAfter", TimeSpan.FromMinutes(2));
    Log.Information("AWS Parameter Store Reload Interval: {ReloadInterval}", reloadAfter);
    builder.Configuration.AddSystemsManager("/shared", reloadAfter);
    builder.Configuration.AddSystemsManager("/production/my-app", reloadAfter);
}
else
{
    Log.Information("AWS Systems Manager integration is disabled");
}

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program), ServiceLifetime.Transient);
builder.Services.ConfigureValidated<Settings1>(builder.Configuration.GetSection("Settings1"));
builder.Services.ConfigureValidated<Settings2>(builder.Configuration.GetSection("Settings2"));
builder.Services.ConfigureValidated<Settings3>(builder.Configuration.GetSection("Settings3"));
builder.Services.AddCustomRuntimeConfiguration(builder.Configuration);

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Configuration.Web v1"));

app.MapGet("/Settings",
    (IConfiguration configuration,
            IOptions<Settings1> options,
            IOptionsSnapshot<Settings1> optionsSnapshot,
            IOptions<Settings2> options2,
            IOptionsSnapshot<Settings3> optionsSnapshot3) =>
        Results.Ok(new
        {
            Settings1 = new
            {
                Options = options.Value,
                Snapshot = optionsSnapshot.Value
            },
            Settings2 = options2.Value,
            Settings3 = optionsSnapshot3.Value,
            Value = configuration.GetValue<string>("MySetting"),
            FileOverrideValue = configuration.GetValue<string>("MySetting2"),
            DeepValue = configuration.GetValue<string>("MySettingStructure:DeepValue"),
            EnvironmentValue = configuration.GetValue<string>("MySetting3:EnvironmentVar"),
            CommandLineValue = configuration.GetValue<string>("MySetting4"),
            SecretValue = configuration.GetValue<string>("MySetting5"),
            RemoteValue = configuration.GetValue<string>("ConnectionStrings:MyConnectionString"),
        }));

app.MapPost("/Settings", ([FromQuery] string value, ICustomRuntimeConfiguration manipulator) =>
{
    manipulator.SetValue("Settings3:DynamicValue", value);
    return Results.Ok();
});

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