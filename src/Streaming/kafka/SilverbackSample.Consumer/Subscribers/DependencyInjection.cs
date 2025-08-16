using Confluent.Kafka;
using Silverback.Messaging.Configuration.Kafka;

namespace SilverbackSample.Consumer.Subscribers;

public static class DependencyInjection
{
    public static IKafkaEndpointsConfigurationBuilder ConfigureBootstrapServers(this IKafkaEndpointsConfigurationBuilder endpoints,
        IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        endpoints.Configure(config => { config.BootstrapServers = configuration.GetValue<string>("ConnectionStrings:messaging")!; });
        return endpoints;
    }

    public static IKafkaConsumerEndpointBuilder ConfigureDefaultClient(this IKafkaConsumerEndpointBuilder endpoint)
    {
        endpoint.Configure(config =>
        {
            config.GroupId = "sample-consumer";
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
        });
        return endpoint;
    }
}