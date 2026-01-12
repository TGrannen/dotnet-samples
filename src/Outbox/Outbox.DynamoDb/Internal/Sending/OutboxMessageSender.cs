using System.Text.Json;
using Outbox.Messaging.Abstractions;

namespace Outbox.DynamoDb.Internal.Sending;

internal class OutboxMessageSender(IMessagePublisher messagePublisher, IDynamoDBContext context) : IOutboxMessageSender
{
    public async Task SendOutboxMessages(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken)
    {
        var batchWrite = context.CreateBatchWrite<OutboxMessage>();
        foreach (var message in messages)
        {
            var domainMessage = JsonSerializer.Deserialize<DomainMessage>(message.Payload!);
            await messagePublisher.PublishAsync(new Message
            {
                Id = domainMessage!.Id,
                Payload = JsonSerializer.Serialize(domainMessage.Payload)
            }, cancellationToken);
            batchWrite.AddDeleteKey(message.Key);
        }

        await context.ExecuteBatchWriteAsync([batchWrite], cancellationToken);
    }
}