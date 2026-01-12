using System.Text.Json;
using Outbox.DynamoDb.Internal.Background;

namespace Outbox.DynamoDb.Internal;

internal class DynamoDbTransaction(IDynamoDBContext context, IOutboxMessageQueue queue) : IDynamoDbTransaction
{
    private readonly List<IBatchWrite> _batchWrites = [];
    private readonly List<OutboxMessage> _messages = [];

    public async Task<List<T>> GetAll<T>(CancellationToken cancellationToken = default)
    {
        return await context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetBatch<T, TC>(List<TC> hashKeys)
    {
        var batchRequest = context.CreateBatchGet<T>();
        foreach (var key in hashKeys)
        {
            batchRequest.AddKey(key);
        }

        await batchRequest.ExecuteAsync();
        return batchRequest.Results;
    }

    public async Task<T> GetByHash<T>(object hashKey, CancellationToken cancellationToken = default)
    {
        return await context.LoadAsync<T>(hashKey, cancellationToken);
    }

    public void Upsert<T>(T item)
    {
        var batchWrite = Get<T>();
        batchWrite.AddPutItem(item);
    }

    public void Delete<T>(T item)
    {
        var batchWrite = Get<T>();
        batchWrite.AddDeleteItem(item);
    }

    public void Delete<T>(object hashKey)
    {
        var batchWrite = Get<T>();
        batchWrite.AddDeleteKey(hashKey);
    }

    public void AddMessage(DomainMessage message)
    {
        var outboxMessage = new OutboxMessage
        {
            Key = message.Id,
            Created = DateTime.Now, // TODO Inject IDateTime interface
            Payload = JsonSerializer.Serialize(message)
        };
        _messages.Add(outboxMessage);
        var batchWrite = Get<OutboxMessage>();
        batchWrite.AddPutItem(outboxMessage);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var singleWrite = context.CreateMultiTableBatchWrite(_batchWrites.ToArray());
        await singleWrite.ExecuteAsync(cancellationToken);
        _batchWrites.Clear();

        foreach (var message in _messages)
        {
            queue.Add(message);
        }

        _messages.Clear();
    }

    private IBatchWrite<T> Get<T>()
    {
        if (_batchWrites.Any(x => x.GetType() == typeof(BatchWrite<T>)))
        {
            return (_batchWrites.First(x => x.GetType() == typeof(BatchWrite<T>)) as BatchWrite<T>)!;
        }

        var batchWrite = context.CreateBatchWrite<T>();
        _batchWrites.Add(batchWrite);
        return batchWrite;
    }
}