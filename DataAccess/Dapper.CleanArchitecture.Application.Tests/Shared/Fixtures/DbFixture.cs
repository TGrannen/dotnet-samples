using Dapper.CleanArchitecture.Domain.Common.Interfaces;

namespace Dapper.CleanArchitecture.Application.Tests.Shared.Fixtures;

public class DbFixture
{
    public Mock<DbConnection> DbConnection { get; } = new();
    public Mock<IDbContext> Context { get; } = new();
    public Mock<IDbReadContext> ReadContext { get; } = new();
}