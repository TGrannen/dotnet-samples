using Serilog;
using WorkerServiceExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ExampleService>();

builder.Services.AddHttpClient();
builder.Services.AddHostedService<HttpPollExample>();

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();

await app.RunAsync();
