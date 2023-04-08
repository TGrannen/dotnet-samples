namespace Outbox.DynamoDb;

public interface IDynamoDbTransaction
{
    Task<List<T>> GetAll<T>(CancellationToken cancellationToken = default);
    Task<T> GetByHash<T>(object hashKey, CancellationToken cancellationToken = default);
    void Upsert<T>(T item);
    void Delete<T>(T item);
    void Delete<T>(object hashKey);
    void AddMessage(DomainMessage message);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class DomainMessage
{
    public Guid Id { get; } = Guid.NewGuid();

    public object? Payload { get; init; }
}