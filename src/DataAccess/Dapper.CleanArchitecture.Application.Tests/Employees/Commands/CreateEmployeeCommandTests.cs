using Dapper.CleanArchitecture.Application.Employees.Commands;

namespace Dapper.CleanArchitecture.Application.Tests.Employees.Commands;

public class CreateEmployeeCommandTests
{
    private readonly ApplicationFixture _fixture = new();
    private const int EmployeeNumber = 451;

    public CreateEmployeeCommandTests()
    {
        _fixture.DbConnection
            .SetupDapperAsync(c => c.ExecuteScalarAsync<int>(It.IsAny<string>(), null, null, null, null))
            .ReturnsAsync(EmployeeNumber);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChanges()
    {
        await _fixture.SendAsync(new CreateEmployeeCommand { EmployeeNumber = EmployeeNumber });
        _fixture.Context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Handle_ShouldReturnDbValueInVm()
    {
        var result = await _fixture.SendAsync(new CreateEmployeeCommand { EmployeeNumber = EmployeeNumber });
        result.ShouldNotBeNull();
        result.EmployeeNumber.ShouldBe(EmployeeNumber);
    }
}