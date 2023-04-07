namespace Outbox.SampleBlazor.Services;

public class FakeMessagePublisher : IMessagePublisher
{
    private readonly ILogger<FakeMessagePublisher> _logger;

    public FakeMessagePublisher(ILogger<FakeMessagePublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(IMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message sent! {Message}", message);

        return Task.CompletedTask;
    }
}