using Outbox.Messaging.Abstractions;

namespace Outbox.DynamoDb.Internal;

internal class DynamoDbTransaction : IDynamoDbTransaction
{
    private readonly IDynamoDBContext _context;
    private readonly IMessagePublisher _messagePublisher;
    private readonly List<BatchWrite> _batchWrites = new();
    private readonly List<IMessage> _messages = new();

    public DynamoDbTransaction(IDynamoDBContext context, IMessagePublisher messagePublisher)
    {
        _context = context;
        _messagePublisher = messagePublisher;
    }

    public async Task<List<T>> GetAll<T>(CancellationToken cancellationToken = default)
    {
        return await _context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetBatch<T, TC>(List<TC> hashKeys)
    {
        var batchRequest = _context.CreateBatchGet<T>();
        foreach (var key in hashKeys)
        {
            batchRequest.AddKey(key);
        }
        await batchRequest.ExecuteAsync();
        return batchRequest.Results;
    }

    public async Task<T> GetByHash<T>(object hashKey, CancellationToken cancellationToken = default)
    {
        return await _context.LoadAsync<T>(hashKey, cancellationToken);
    }
    public void Upsert<T>(T item)
    {
        var batchWrite = Get<T>();
        batchWrite.AddPutItem(item);
    }

    public void Delete<T>(object hashKey)
    {
        var batchWrite = Get<T>();
        batchWrite.AddDeleteKey(hashKey);
    }

    public void AddMessage(IMessage message)
    {
        _messages.Add(message);
        var batchWrite = Get<OutboxMessage>();
        batchWrite.AddPutItem(new OutboxMessage
        {
            Key = message.Id,
            Created = DateTime.Now, // TODO Inject IDateTime interface
            Message = message
        });
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var singleWrite = _context.CreateMultiTableBatchWrite(_batchWrites.ToArray());
        await singleWrite.ExecuteAsync(cancellationToken);
        _batchWrites.Clear();

        // TODO Wrap in retry?
        foreach (var message in _messages)
        {
            await _messagePublisher.PublishAsync(message, cancellationToken);
        }
    }

    private BatchWrite<T> Get<T>()
    {
        if (_batchWrites.Any(x => x.GetType() == typeof(BatchWrite<T>)))
        {
            return (_batchWrites.First(x => x.GetType() == typeof(BatchWrite<T>)) as BatchWrite<T>)!;
        }

        var batchWrite = _context.CreateBatchWrite<T>();
        _batchWrites.Add(batchWrite);
        return batchWrite;
    }
}