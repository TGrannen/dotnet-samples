namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbContext : IDbContext, IDisposable
{
    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;
    private readonly ILogger<DbContext> _logger;
    private readonly List<IDomainEvent> _events = new();
    private IDbTransaction _transaction = null;

    public DbContext(IDbConnectionProvider connection, IPublisher publisher, ILogger<DbContext> logger)
    {
        _connection = connection.Connection;
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
            _transaction.Dispose();
            _transaction = null;
            _logger.LogDebug("DB Transaction committed successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during DBContext SaveChangesAsync. Rolling back transaction");
            _transaction?.Rollback();
            throw;
        }

        if (_events.Any())
        {
            await PublishEvents(token);
        }
    }

    private async Task PublishEvents(CancellationToken token)
    {
        _logger.LogDebug("Firing off Domain Events: {@Events}", _events);

        // Prevents adding to the same collection while it's being iterated over.
        // Another solution could be creating separate scopes for each Notification Publish but then
        // they would each get their own DB connection
        var publishList = _events.ToList();
        _events.Clear();

        foreach (var domainEvent in publishList)
        {
            await _publisher.Publish(domainEvent, token);
        }
    }

    public void Dispose()
    {
        if (_transaction == null)
        {
            return;
        }

        _logger.LogDebug("Disposing of DB Transaction");
        _transaction.Dispose();
        GC.SuppressFinalize(this);
    }
}