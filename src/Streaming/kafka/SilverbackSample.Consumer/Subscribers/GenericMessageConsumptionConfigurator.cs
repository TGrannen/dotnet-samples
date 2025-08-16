namespace SilverbackSample.Consumer.Subscribers;

public class GenericMessageConsumptionConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder.AddKafkaEndpoints(endpoints => endpoints
            .ConfigureBootstrapServers(builder.ServiceProvider)
            .AddInbound(endpoint => endpoint
                .ConsumeFrom("samples-basic")
                .ConfigureDefaultClient()));
    }
}