namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbContext : IDbContext
{
    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;
    private readonly ILogger<DbContext> _logger;
    private readonly List<IDomainEvent> _events = new();
    private IDbTransaction _transaction;

    public DbContext(IDbConnection connection, IPublisher publisher, ILogger<DbContext> logger)
    {
        _connection = connection;
        _publisher = publisher;
        _logger = logger;
    }

    public IDbConnection Connection
    {
        get
        {
            if (_transaction != null)
                return _transaction.Connection;

            _logger.LogDebug("Creating new DB Transaction");
            _transaction = _connection.BeginTransaction();

            return _transaction.Connection;
        }
    }

    public void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("DBContext SaveChangesAsync was called but no transaction currently exists");
            return;
        }

        try
        {
            _logger.LogDebug("Committing DB Transaction");
            _transaction.Commit();
            _transaction = null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during DBContext SaveChangesAsync. Rolling back transaction");
            _transaction?.Rollback();
            throw;
        }

        _logger.LogDebug("Firing off Domain Events: {@Events}", _events);
        foreach (var domainEvent in _events)
        {
            await _publisher.Publish(domainEvent, token);
        }
    }
}