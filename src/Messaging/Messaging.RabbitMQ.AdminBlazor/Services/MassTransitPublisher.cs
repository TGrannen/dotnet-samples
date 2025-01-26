namespace Messaging.RabbitMQ.AdminBlazor.Services;

class MassTransitPublisher : IPublisher
{
    private readonly IBus _bus;

    public MassTransitPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T message)
    {
        await _bus.Publish(message);
    }
}