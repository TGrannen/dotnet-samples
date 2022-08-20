using System.Data;
using Npgsql;

namespace Dapper.Web.Services;

public interface IConnectionProvider
{
    public IDbConnection Connection { get; }
}

class ConnectionProvider : IConnectionProvider
{
    private readonly IConnectionStringProvider _connectionStringProvider;
    private IDbConnection? _connection;

    public ConnectionProvider(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public IDbConnection Connection
    {
        get
        {
            if (_connection != null)
            {
                return _connection;
            }

            _connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString);
            _connection.Open();
            return _connection;
        }
    }
}