namespace Outbox.Messaging.Abstractions;

public interface IMessagePublisher
{
    Task PublishAsync(Message message, CancellationToken cancellationToken = default);
}