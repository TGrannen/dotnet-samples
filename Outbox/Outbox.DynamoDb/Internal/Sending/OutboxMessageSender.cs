using System.Text.Json;
using Outbox.Messaging.Abstractions;

namespace Outbox.DynamoDb.Internal.Sending;

internal class OutboxMessageSender : IOutboxMessageSender
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IDynamoDBContext _context;

    public OutboxMessageSender(IMessagePublisher messagePublisher, IDynamoDBContext context)
    {
        _messagePublisher = messagePublisher;
        _context = context;
    }

    public async Task SendOutboxMessages(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken)
    {
        var batchWrite = _context.CreateBatchWrite<OutboxMessage>();
        foreach (var message in messages)
        {
            var domainMessage = JsonSerializer.Deserialize<DomainMessage>(message.Payload!);
            await _messagePublisher.PublishAsync(new Message
            {
                Id = domainMessage.Id,
                Payload = JsonSerializer.Serialize(domainMessage.Payload)
            }, cancellationToken);
            batchWrite.AddDeleteKey(message.Key);
        }

        await _context.ExecuteBatchWriteAsync(new BatchWrite[] { batchWrite }, cancellationToken);
    }
}