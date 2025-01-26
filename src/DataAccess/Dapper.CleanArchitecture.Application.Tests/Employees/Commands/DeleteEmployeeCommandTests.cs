using Dapper.CleanArchitecture.Application.Employees.Commands;
using Dapper.CleanArchitecture.Domain.Employees.Notifications;

namespace Dapper.CleanArchitecture.Application.Tests.Employees.Commands;

public class DeleteEmployeeCommandTests
{
    private readonly ApplicationFixture _fixture = new();
    private const int EmployeeNumber = 451;

    public DeleteEmployeeCommandTests()
    {
        _fixture.DbConnection
            .SetupDapperAsync(c => c.ExecuteScalarAsync<int>(It.IsAny<string>(), null, null, null, null))
            .ReturnsAsync(EmployeeNumber);
    }

    [Fact]
    public async Task Handle_ShouldAddDomainEvent()
    {
        await _fixture.SendAsync(new DeleteEmployeeCommand { EmployeeNumber = EmployeeNumber });
        _fixture.Context.Verify(x => x.AddEvent(It.IsAny<EmployeeDeletedEvent>()));
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChanges()
    {
        await _fixture.SendAsync(new DeleteEmployeeCommand { EmployeeNumber = EmployeeNumber });
        _fixture.Context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Handle_ShouldNotThrowException()
    {
        await _fixture.SendAsync(new DeleteEmployeeCommand { EmployeeNumber = EmployeeNumber });
    }
}