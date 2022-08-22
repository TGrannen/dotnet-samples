using Dapper.CleanArchitecture.Domain.Common.Interfaces;

namespace Dapper.CleanArchitecture.Domain.Employees.Notifications;

public record EmployeeUpdatedEvent : IDomainEvent
{
    public int EmployeeNumber { get; init; }
}