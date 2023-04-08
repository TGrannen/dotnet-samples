namespace Outbox.SampleBlazor.Services;

public class FakeMessagePublisher : IMessagePublisher
{
    private readonly ILogger<FakeMessagePublisher> _logger;
    private readonly MessagePublisherSettings _settings;
    private readonly ReloadEventProducer _reloadEventProducer;

    public FakeMessagePublisher(ILogger<FakeMessagePublisher> logger, MessagePublisherSettings settings,
        ReloadEventProducer reloadEventProducer)
    {
        _logger = logger;
        _settings = settings;
        _reloadEventProducer = reloadEventProducer;
    }

    public async Task PublishAsync(Message message, CancellationToken cancellationToken)
    {
        if (_settings.ShouldThrowException)
        {
            throw new Exception("Failure to send message " + message.Id);
        }

        await Task.Delay(TimeSpan.FromMilliseconds(_settings.PublishDelay), cancellationToken);
        _logger.LogInformation("Message sent! {@Message}", message);
        _reloadEventProducer.SendReload();
    }
}