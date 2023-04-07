namespace Outbox.Messaging.Abstractions;

public interface IMessagePublisher
{
    Task PublishAsync(IMessage message, CancellationToken cancellationToken = default);
}