using Outbox.Messaging.Abstractions;

namespace Outbox.DynamoDb;

public interface IDynamoDbTransaction
{
    Task<List<T>> GetAll<T>(CancellationToken cancellationToken = default);
    Task<T> GetByHash<T>(object hashKey, CancellationToken cancellationToken = default);
    void Upsert<T>(T item);
    void Delete<T>(object hashKey);
    void AddMessage(IMessage message);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}