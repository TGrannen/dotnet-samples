namespace Messaging.RabbitMQ.AdminBlazor.Services;

public interface IPublisher
{
    Task InvokeAsync(object message, CancellationToken token);
}

class MassTransitPublisher : IPublisher
{
    private readonly IBus _bus;

    public MassTransitPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task InvokeAsync(object message, CancellationToken token)
    {
        await _bus.Publish(message, token);
    }
}