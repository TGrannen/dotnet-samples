namespace Messaging.RabbitMQ.AdminBlazor.Services;

public class PublisherBackgroundServiceConfig
{
    public bool ShouldPublish { get; set; } = false;
    public int RateSeconds { get; set; } = 2;
}

public class PublisherBackgroundService : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<PublisherBackgroundService> _logger;
    private readonly PublisherBackgroundServiceConfig _config;

    public PublisherBackgroundService(IBus bus, ILogger<PublisherBackgroundService> logger, PublisherBackgroundServiceConfig config)
    {
        _bus = bus;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_config.ShouldPublish)
            {
                var message = new TestMessage { MyId = Guid.NewGuid(), Time = DateTime.Now };
                _logger.LogInformation("Publishing message {@Message}", message);
                await _bus.Publish(message, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.RateSeconds), stoppingToken);
        }
    }
}