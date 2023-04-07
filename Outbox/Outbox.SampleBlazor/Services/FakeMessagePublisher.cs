namespace Outbox.SampleBlazor.Services;

public class FakeMessagePublisher : IMessagePublisher
{
    private readonly ILogger<FakeMessagePublisher> _logger;
    private readonly MessagePublisherSettings _settings;

    public FakeMessagePublisher(ILogger<FakeMessagePublisher> logger, MessagePublisherSettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task PublishAsync(Message message, CancellationToken cancellationToken)
    {
        if (_settings.ShouldThrowException)
        {
            throw new Exception("Failure to send message " + message.Id);
        }

        await Task.Delay(TimeSpan.FromMilliseconds(_settings.PublishDelay), cancellationToken);
        _logger.LogInformation("Message sent! {@Message}", message);
    }
}