namespace GettingStarted.Consumers;

public class HelloConsumer(ILogger<HelloConsumer> logger) : IConsumer<Hello>
{
    public Task Consume(ConsumeContext<Hello> context)
    {
        logger.LogInformation("Hello {Name}", context.Message.Value);
        return Task.CompletedTask;
    }
}