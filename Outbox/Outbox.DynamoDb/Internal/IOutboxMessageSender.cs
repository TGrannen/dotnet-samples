namespace Outbox.DynamoDb.Internal;

internal interface IOutboxMessageSender
{
    Task SendOutboxMessages(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken);
}