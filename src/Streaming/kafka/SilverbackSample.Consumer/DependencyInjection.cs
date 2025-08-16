namespace SilverbackSample.Consumer;

public static class DependencyInjection
{
    public static ISilverbackBuilder GenericSampleMessageConsumption(this IServiceCollection services)
    {
        return services
            .ConfigureSilverback()
            .AddEndpointsConfigurator<GenericMessageConsumptionConfigurator>();
    }

    public static void AddSampleMessageBatching(this IServiceCollection services)
    {
        services
            .ConfigureSilverback()
            .AddEndpointsConfigurator<SampleMessageBatchSubscriberConfigurator>()
            .AddScopedSubscriber<SampleMessageBatchSubscriber>();
    }
}