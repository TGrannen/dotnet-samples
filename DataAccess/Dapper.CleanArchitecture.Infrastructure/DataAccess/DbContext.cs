using Dapper.CleanArchitecture.Domain.Common.Interfaces;

namespace Dapper.CleanArchitecture.Infrastructure.DataAccess;

public class DbContext : IDbContext
{
    public DbContext(IDbConnection connection)
    {
        Connection = connection;
    }

    public IDbConnection Connection { get; }

    public void AddEvent(IDomainEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}