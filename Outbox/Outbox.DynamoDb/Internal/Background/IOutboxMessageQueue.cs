using System.Collections.Concurrent;

namespace Outbox.DynamoDb.Internal.Background;

internal interface IOutboxMessageQueue
{
    void Add(OutboxMessage message);
    bool IsEmpty();
    OutboxMessage[] Take(int size = 10);
}

class OutboxMessageQueue : IOutboxMessageQueue
{
    private readonly ConcurrentQueue<OutboxMessage> _queue = new();

    public void Add(OutboxMessage message)
    {
        _queue.Enqueue(message);
    }

    public bool IsEmpty()
    {
        return _queue.IsEmpty;
    }

    public OutboxMessage[] Take(int size = 10)
    {
        if (_queue.IsEmpty)
        {
            return Array.Empty<OutboxMessage>();
        }

        var list = new List<OutboxMessage>();
        while (_queue.TryDequeue(out var value))
        {
            list.Add(value);
            if (list.Count >= size)
            {
                break;
            }
        }

        return list.ToArray();
    }
}