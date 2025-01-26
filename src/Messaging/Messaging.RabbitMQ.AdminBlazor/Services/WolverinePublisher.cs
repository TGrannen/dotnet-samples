using Wolverine;

namespace Messaging.RabbitMQ.AdminBlazor.Services;

class WolverinePublisher : IPublisher
{
    private readonly IMessageBus  _bus;

    public WolverinePublisher(IMessageBus  bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T message)
    {
        await _bus.PublishAsync(message);
    }
}