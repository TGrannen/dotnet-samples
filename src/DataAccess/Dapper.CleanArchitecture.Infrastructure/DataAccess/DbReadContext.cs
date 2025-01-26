namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public sealed class DbReadContext : IDbReadContext, IDisposable
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DbReadContext> _logger;
    private IDbConnection _connection;

    public DbReadContext(IDbConnectionFactory connectionFactory, ILogger<DbReadContext> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public IDbConnection Connection => _connection ??= _connectionFactory.CreateDbConnection();

    public void Dispose()
    {
        if (_connection == null)
        {
            return;
        }

        _logger.LogDebug("Disposing of DB Connection");
        _connection.Dispose();
    }
}