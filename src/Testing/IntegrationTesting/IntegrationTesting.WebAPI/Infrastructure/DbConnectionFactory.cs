using System.Data;
using Npgsql;

namespace IntegrationTesting.WebAPI.Infrastructure;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("DbConnectionString");
        return new NpgsqlConnection(connectionString);
    }
}