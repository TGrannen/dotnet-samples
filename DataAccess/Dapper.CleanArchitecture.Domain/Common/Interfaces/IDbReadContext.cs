namespace Dapper.CleanArchitecture.Domain.Common.Interfaces;

public interface IDbReadContext
{
    public IDbConnection Connection { get; }
}