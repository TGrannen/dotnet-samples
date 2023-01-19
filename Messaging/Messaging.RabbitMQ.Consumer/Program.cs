using MassTransit;
using Messaging.RabbitMQ.Consumer.Consumers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TestMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // cfg.UseMessageRetry(r => r.Immediate(5));
        cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));

        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
await app.RunAsync();

await Log.CloseAndFlushAsync();