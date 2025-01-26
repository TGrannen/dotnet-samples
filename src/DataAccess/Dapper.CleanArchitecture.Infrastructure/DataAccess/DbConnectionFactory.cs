namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

internal class DbConnectionFactory : IDbConnectionFactory
{
    private readonly ILogger<DbConnectionFactory> _logger;
    private readonly IDbConnectionStringProvider _connectionStringProvider;

    public DbConnectionFactory(ILogger<DbConnectionFactory> logger, IDbConnectionStringProvider connectionStringProvider)
    {
        _logger = logger;
        _connectionStringProvider = connectionStringProvider;
    }

    public IDbConnection CreateDbConnection()
    {
        _logger.LogDebug("Opening a new DB Connection");
        var connection = new NpgsqlConnection(_connectionStringProvider.GetConnectionString());
        connection.Open();
        return connection;
    }
}