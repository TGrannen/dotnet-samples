using MassTransit;
using Messaging.AmazonSQS.ConsumerB.Consumers;
using Messaging.AmazonSQS.Extensions;
using Messaging.Configuration;
using Messaging.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets(typeof(Program).Assembly)
    .AddEnvironmentVariables()
    .Build();

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, config) => { config.ReadFrom.Configuration(context.Configuration); })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.UsingAmazonSqs((context, cfg) =>
            {
                var config = context.GetRequiredService<IAwsSqsConfig>();
                cfg.Host(config.HostName, h =>
                {
                    h.AccessKey(config.AccessKey);
                    h.SecretKey(config.SecretKey);
                });

                cfg.AddAwsTopicAndQueueEndpoint("fancy-producer-topic", "consumer-B-queue",
                    e => { e.Consumer(context.GetRequiredService<ValueChangedConsumerB>); });
            });
        });

        services.AddAwsSqsConfiguration(configuration);
        services.AddTransient<ValueChangedConsumerB>();

        services.AddMassTransitHostedService();
    });

await hostBuilder.RunConsoleAsync();