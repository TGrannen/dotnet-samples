namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbReadContext : IDbReadContext
{
    public DbReadContext(IDbConnection connection)
    {
        Connection = connection;
    }

    public IDbConnection Connection { get; }
}