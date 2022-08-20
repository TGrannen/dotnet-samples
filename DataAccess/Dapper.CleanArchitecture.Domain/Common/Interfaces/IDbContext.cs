namespace Dapper.CleanArchitecture.Domain.Common.Interfaces;

public interface IDbContext : IDbReadContext
{
    void AddEvent(IDomainEvent @event);
    Task SaveChangesAsync(CancellationToken token = default);
}