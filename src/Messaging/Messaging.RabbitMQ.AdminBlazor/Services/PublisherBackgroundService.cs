namespace Messaging.RabbitMQ.AdminBlazor.Services;

public class PublisherBackgroundServiceConfig
{
    public bool ShouldPublish { get; set; } = false;
    public bool Fail { get; set; } = false;
    public int RateSeconds { get; set; } = 2;
    public int DelayMilliSeconds { get; set; } = 500;
}

public class PublisherBackgroundService : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly ILogger<PublisherBackgroundService> _logger;
    private readonly PublisherBackgroundServiceConfig _config;

    public PublisherBackgroundService(IPublisher publisher, ILogger<PublisherBackgroundService> logger, PublisherBackgroundServiceConfig config)
    {
        _publisher = publisher;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_config.ShouldPublish)
            {
                var message = new TestMessage
                {
                    MyId = Guid.NewGuid(),
                    Time = DateTime.Now,
                    Delay = TimeSpan.FromMilliseconds(_config.DelayMilliSeconds),
                    ToFail = _config.Fail
                };
                _logger.LogInformation("Publishing message {@Message}", message);
                await _publisher.PublishAsync(message);
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.RateSeconds), stoppingToken);
        }
    }
}