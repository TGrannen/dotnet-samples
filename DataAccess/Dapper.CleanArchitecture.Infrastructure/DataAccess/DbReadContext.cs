namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbReadContext : IDbReadContext
{
    public DbReadContext(IDbConnectionProvider connection)
    {
        Connection = connection.Connection;
    }

    public IDbConnection Connection { get; }
}