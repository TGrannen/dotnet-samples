namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbContext : IDbContext
{
    private readonly IPublisher _publisher;
    private readonly List<IDomainEvent> _events = new();

    public DbContext(IDbConnection connection, IPublisher publisher)
    {
        _publisher = publisher;
        Connection = connection;
    }

    public IDbConnection Connection { get; }

    public void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        foreach (var domainEvent in _events)
        {
            await _publisher.Publish(domainEvent, token);
        }
    }
}