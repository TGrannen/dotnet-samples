namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

internal class DbConnectionProvider : IDbConnectionProvider, IDisposable
{
    private readonly ILogger<DbConnectionProvider> _logger;

    public DbConnectionProvider(ILogger<DbConnectionProvider> logger, IDbConnectionStringProvider connectionStringProvider)
    {
        _logger = logger;

        _logger.LogDebug("Opening a new DB Connection");
        var connection = new NpgsqlConnection(connectionStringProvider.GetConnectionString());
        connection.Open();
        Connection = connection;
    }

    public IDbConnection Connection { get; }

    public void Dispose()
    {
        _logger.LogDebug("Disposing of DB Connection");
        Connection?.Dispose();
    }
}