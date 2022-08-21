namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;

public interface IDbConnectionProvider
{
    IDbConnection Connection { get; }
}