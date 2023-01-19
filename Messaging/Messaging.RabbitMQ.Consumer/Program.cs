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
        // cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
        
        //https://masstransit-project.com/advanced/middleware/killswitch.html
        cfg.UseKillSwitch(options => options
            .SetActivationThreshold(1)
            .SetTripThreshold(0.01)
            .SetRestartTimeout(s: 10));
        
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
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