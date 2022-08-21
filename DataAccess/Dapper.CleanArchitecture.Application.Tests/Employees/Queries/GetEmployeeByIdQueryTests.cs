using System;
using Dapper.CleanArchitecture.Application.Employees.Queries;
using Dapper.CleanArchitecture.Domain.Employees;

namespace Dapper.CleanArchitecture.Application.Tests.Employees.Queries;

public class GetEmployeeByIdQueryTests
{
    private readonly ApplicationFixture _fixture = new();
    private const int EmployeeNumber = 451;

    public GetEmployeeByIdQueryTests()
    {
        _fixture.DbConnection
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Employee>(It.IsAny<string>(), null, null, null, null))
            .ReturnsAsync(new Employee());
    }

    [Fact]
    public async Task Handle_ShouldReturnDbVm()
    {
        var employee = new Employee
        {
            EmpNo = 451,
            BirthDate = DateTime.Now,
            FirstName = "TEST",
            HireDate = DateTime.Now.AddDays(-4),
            LastName = "WOW"
        };
        _fixture.DbConnection
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Employee>(It.IsAny<string>(), null, null, null, null))
            .ReturnsAsync(employee);
        var result = await _fixture.SendAsync(new GetEmployeeByIdQuery { EmployeeNumber = EmployeeNumber });
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(employee);
    }
}