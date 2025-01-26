using Dapper.CleanArchitecture.Application.Employees.Commands;
using Dapper.CleanArchitecture.Web.Tests.Integration.Shared;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Employees;

[Collection(nameof(WebFactoryCollection))]
public class EmployeeCrudApiTests : TestsBase
{
    private readonly HttpClient _client;
    private readonly IDbReadContext _dbService;

    public EmployeeCrudApiTests(AppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });

        _dbService = factory.Services.CreateScope().ServiceProvider.GetRequiredService(typeof(IDbReadContext)) as IDbReadContext;
    }

    [Fact]
    public async Task Create_ShouldReturn200_WhenCreatingANewEmployee()
    {
        var command = new CreateEmployeeCommand
        {
            EmployeeNumber = 45124,
            BirthDate = DateTime.Now,
            HireDate = DateTime.Now.AddDays(4),
            FirstName = "BOB",
            LastName = "Jones",
        };

        var employeeNumber = await GetEmployeeNumberFromTable(command.EmployeeNumber);
        employeeNumber.Should().Be(0);

        var response = await _client.PostAsJsonAsync("/Clean/Create", command);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        employeeNumber = await GetEmployeeNumberFromTable(command.EmployeeNumber);
        employeeNumber.Should().Be(command.EmployeeNumber);
    }

    private async Task<int> GetEmployeeNumberFromTable(int number)
    {
        return await _dbService.Connection.QueryFirstOrDefaultAsync<int>("Select emp_no from employees where emp_no = @Number",
            new { Number = number });
    }
}