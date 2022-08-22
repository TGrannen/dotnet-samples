namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateDbConnection();
}