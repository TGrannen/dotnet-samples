using Polly.ConsoleApp;
using Polly.ConsoleApp.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(builder.Configuration); });

builder.Services.AddSingleton<TestService>();
builder.Services.AddSQSSendMessagePipeline(builder.Configuration);
builder.Services.Configure<TestConfig>(builder.Configuration.GetSection("TestConfig"));

var app = builder.Build();

Log.Information("Starting App");
var service = app.Services.GetRequiredService<TestService>();
try
{
    await service.Handle(app.Lifetime.ApplicationStopping);
}
catch (Exception e)
{
    Log.Fatal(e, "Service method threw and exception");
}

Log.Information("App Stopped");
Log.CloseAndFlush();