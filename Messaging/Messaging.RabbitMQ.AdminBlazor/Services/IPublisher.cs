namespace Messaging.RabbitMQ.AdminBlazor.Services;

public interface IPublisher
{
    Task PublishAsync<T>(T message);
}