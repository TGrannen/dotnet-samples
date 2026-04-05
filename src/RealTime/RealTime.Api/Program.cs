using RealTime.Api.Hubs;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:7024",
                "http://localhost:5204")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var redisConnection = builder.Configuration.GetConnectionString("redis");
var signalR = builder.Services.AddSignalR();
if (!string.IsNullOrEmpty(redisConnection))
{
    signalR.AddStackExchangeRedis(redisConnection, options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("realtime-demo");
    });
}

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Text(
    "RealTime.Api — SignalR hub at /chathub (Redis backplane when run via Aspire).",
    "text/plain"));

app.MapHub<ChatHub>("/chathub");

app.Run();
