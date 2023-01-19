using JasperFx.Core;
using MassTransit;
using Messaging.RabbitMQ.AdminBlazorContracts.Models;
using Messaging.RabbitMQ.Consumer.Consumers;
using Serilog;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();
var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:Host");
bool useWolverine = builder.Configuration.GetValue("UseWolverine", false);
if (useWolverine)
{
    builder.Host.UseWolverine(opts =>
    {
        opts.ServiceName = "Subscriber1";
        // opts.Handlers.OnException<Exception>().MoveToErrorQueue();
        
        // The failing message is requeued for later processing, then
        // the specific listener is paused for 10 minutes
        opts.Handlers.OnException<Exception>() //SystemIsCompletelyUnusableException
            .Requeue().AndPauseProcessing(20.Seconds());
        
        opts.UseRabbitMq(rabbit => { rabbit.HostName = rabbitHost; })
            .DeclareExchange("test-message", exchange =>
            {
                exchange.BindQueue("consumer-test-message");
            })
            .AutoProvision();

        opts.ListenToRabbitQueue("consumer-test-message", queue => queue.TimeToLive(15.Seconds()));
    });
}
else
{
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
}

var app = builder.Build();
await app.RunAsync();

await Log.CloseAndFlushAsync();