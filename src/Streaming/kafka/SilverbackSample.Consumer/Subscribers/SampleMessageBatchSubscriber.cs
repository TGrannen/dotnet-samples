namespace SilverbackSample.Consumer.Subscribers;

public class SampleMessageBatchSubscriber(ILogger<SampleMessageBatchSubscriber> logger)
{
    public async Task OnBatchReceivedAsync(IAsyncEnumerable<SampleBatchMessage> batch)
    {
        var sum = 0;
        var count = 0;

        await foreach (var message in batch)
        {
            sum += message.Number;
            count++;
        }

        logger.LogInformation(
            "Received batch of {Count} message -> sum: {Sum}",
            count,
            sum);
    }
}

public class SampleMessageBatchSubscriberConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder.AddKafkaEndpoints(endpoints => endpoints
            .ConfigureBootstrapServers(builder.ServiceProvider)
            .AddInbound(endpoint => endpoint
                .ConsumeFrom("samples-batch")
                .ConfigureDefaultClient()
                .EnableBatchProcessing(100, TimeSpan.FromSeconds(5))));
    }
}