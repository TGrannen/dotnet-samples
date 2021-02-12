using MassTransit;
using Messaging.AmazonSQS.ConsumerB.Consumers;
using Messaging.AmazonSQS.Extensions;
using Messaging.Configuration;
using Messaging.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.AmazonSQS.ConsumerB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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

                    cfg.AddAwsTopicAndQueueEndpoint("fancy-producer-topic", "consumer-B-queue", e =>
                    {
                        e.Consumer(context.GetRequiredService<ValueChangedConsumerB>);
                    });
                });
            });

            services.AddMassTransitHostedService();
            services.AddAwsSqsConfiguration(Configuration);
            services.AddTransient<ValueChangedConsumerB>();
        }

        public void Configure()
        {
        }
    }
}