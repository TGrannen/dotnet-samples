namespace Outbox.DynamoDb.Internal.Sending;

internal interface IOutboxMessageSender
{
    Task SendOutboxMessages(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken);
}