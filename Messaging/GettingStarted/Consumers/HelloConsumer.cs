namespace GettingStarted.Consumers;

public class HelloConsumer(ILogger<HelloConsumer> logger, IOptionsSnapshot<TestingConfig> optionsSnapshot) : IConsumer<Hello>
{
    public async Task Consume(ConsumeContext<Hello> context)
    {
        var config = optionsSnapshot.Value.Hello;

        await Task.Delay(config.Delay, context.CancellationToken);

        if (config.Throw)
        {
            throw new Exception($"Dummy test{DateTime.Now}");
        }

        logger.LogInformation("Hello {Name}", context.Message.Value);
    }
}

public class HelloConsumerDefinition(IOptions<TestingConfig> options) : ConsumerDefinition<HelloConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpoint,
        IConsumerConfigurator<HelloConsumer> consumer,
        IRegistrationContext context)
    {
        endpoint.UseConcurrencyLimit(1);
        endpoint.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));
        endpoint.UseRateLimit(100, TimeSpan.FromSeconds(10));
        //
        // consumer.UseMessageRetry(r =>
        // {
        //     r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
        // });
        consumer.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod = TimeSpan.FromSeconds(30);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 2;
            cb.ResetInterval = options.Value.BackoffDelay;
        });
    }
}