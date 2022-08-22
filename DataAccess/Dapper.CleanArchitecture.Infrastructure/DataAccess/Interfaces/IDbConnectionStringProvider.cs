namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;

public interface IDbConnectionStringProvider
{
    string GetConnectionString();
}