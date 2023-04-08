using System.Collections.Concurrent;

namespace Outbox.DynamoDb.Internal.Background;

internal interface IOutboxMessageQueue
{
    void Add(OutboxMessage message);
    OutboxMessage? TryDequeue();
}

class OutboxMessageQueue : IOutboxMessageQueue
{
    private readonly ConcurrentQueue<OutboxMessage> _queue = new();

    public void Add(OutboxMessage message)
    {
        _queue.Enqueue(message);
    }

    public OutboxMessage? TryDequeue()
    {
        return _queue.TryDequeue(out var value) ? value : null;
    }
}