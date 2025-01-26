using System.Data;
using Npgsql;

namespace IntegrationTesting.WebAPI.Infrastructure;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DbConnectionString");
        return new NpgsqlConnection(connectionString);
    }
}