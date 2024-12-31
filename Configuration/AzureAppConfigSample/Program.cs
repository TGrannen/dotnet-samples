using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.Extensions.Configuration.AzureAppConfiguration", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestSettings"));
var connectionString = builder.Configuration.GetConnectionString("AppConfig");
var serviceName = builder.Configuration.GetValue("AppConfig:ServiceName", "Not-Set");

builder.Services.AddFeatureManagement();
builder.Services.AddAzureAppConfiguration();
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
        .Select("*")
        .Select("*", serviceName)
        .ConfigureRefresh(refreshOptions =>
        {
            refreshOptions.Register("AppConfig:Sentinel", refreshAll: true).SetRefreshInterval(TimeSpan.FromSeconds(15));
        });
    options.ConfigureKeyVault(keyVaultOptions => { keyVaultOptions.SetCredential(new DefaultAzureCredential()); });
    options.UseFeatureFlags(flagOptions => flagOptions.SetRefreshInterval(TimeSpan.FromSeconds(15)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var changeToken = ((IConfiguration)builder.Configuration).GetReloadToken();
changeToken.RegisterChangeCallback(state => { Log.Information("Configuration has changed: {@Data}", state); }, null);

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAzureAppConfiguration();

app.MapGet("/Settings", async (IConfiguration configuration,
        IFeatureManager featureManager,
        IOptions<Settings> options,
        IOptionsSnapshot<Settings> optionsSnapshot,
        IOptionsMonitor<Settings> optionsMonitor) =>
    Results.Ok(new
    {
        Settings1 = new
        {
            Options = options.Value,
            Snapshot = optionsSnapshot.Value,
            Monitor = optionsMonitor.CurrentValue
        },
        RemoteValue = configuration.GetValue<string>("ConnectionStrings:MyConnectionString"),
        FeatureFlag = await featureManager.IsEnabledAsync("TestFeature")
    }));

app.MapPost("/Refresh", async ([FromServices] IConfigurationRefresherProvider configurationRefresher) =>
{
    var result = new List<bool>();
    foreach (var refresher in configurationRefresher.Refreshers)
    {
        var success = await refresher.TryRefreshAsync();
        result.Add(success);
    }

    return Results.Ok(result);
});

app.Run();

public class Settings
{
    public string Value { get; init; }
    public string Value2 { get; init; }
}