using FlurlAPIAccess.TestWebAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<APIConfig>(builder.Configuration.GetSection("APIConfig"));
builder.Services.AddTransient<ITestService, TestService>();
builder.Services.AddTransient<ITestService2, TestService2>();
builder.Services.AddTransient<IFlurlRequestProvider, FlurlRequestProvider>();

// All calls to APIConfig:BaseUrl will use the same HttpClient instance.
FlurlHttp.ConfigureClient(builder.Configuration["APIConfig:BaseUrl"], cli => cli
    .Configure(settings =>
    {
        // keeps logging & error handling out of TestService
        settings.BeforeCall = call =>
            Log.Logger.Information("Calling {Method} {CallRequest}", call.HttpRequestMessage.Method.Method, call.Request.Url);
        settings.OnError = call => Log.Logger.Error(call.Exception, "Call to APIConfig:BaseUrl failed: {CallRequest}", call.Request);
    })
    // adds default headers to send with every call
    .WithHeaders(new
    {
        Accept = "application/json"
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (int id, ITestService service) => await service.GetData(id))
    .WithOpenApi();
app.MapGet("/test2", async (int id, ITestService2 service) => await service.GetData(id))
    .WithOpenApi();
//
// app.MapGet("/test2", async (int id, ITestService2 service) => await service.GetData(id))
//     .WithOpenApi();

app.Run();