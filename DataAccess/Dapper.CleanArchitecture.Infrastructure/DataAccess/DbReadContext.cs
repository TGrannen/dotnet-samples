namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbReadContext : IDbReadContext
{
    public DbReadContext(IDbConnectionFactory connection)
    {
        Connection = connection.CreateDbConnection();
    }

    public IDbConnection Connection { get; }
}