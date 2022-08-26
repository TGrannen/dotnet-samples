namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public sealed class DbReadContext : IDbReadContext, IDisposable
{
    private readonly ILogger<DbReadContext> _logger;

    public DbReadContext(IDbConnectionFactory connectionFactory, ILogger<DbReadContext> logger)
    {
        _logger = logger;
        Connection = connectionFactory.CreateDbConnection();
    }

    public IDbConnection Connection { get; }

    public void Dispose()
    {
        if (Connection == null)
        {
            return;
        }

        _logger.LogDebug("Disposing of DB Connection");
        Connection.Dispose();
    }
}