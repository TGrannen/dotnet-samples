using MassTransit;
using Messaging.AmazonSQS.ConsumerA.Consumers;
using Messaging.AmazonSQS.Extensions;
using Messaging.Configuration;
using Messaging.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.AmazonSQS.ConsumerA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
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

                    cfg.AddAwsTopicAndQueueEndpoint("fancy-producer-topic", "consumer-A-queue", e =>
                    {
                        e.Consumer(context.GetRequiredService<ValueChangedConsumerA>);
                        e.Consumer(context.GetRequiredService<RecordCreatedConsumerA>);
                    });
                });
            });

            services.AddMassTransitHostedService();
            services.AddAwsSqsConfiguration(Configuration);
            services.AddTransient<ValueChangedConsumerA>();
            services.AddTransient<RecordCreatedConsumerA>();
        }

        public void Configure()
        {
        }
    }
}