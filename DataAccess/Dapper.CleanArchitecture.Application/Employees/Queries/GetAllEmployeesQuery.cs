using Dapper.CleanArchitecture.Domain.Employees;

namespace Dapper.CleanArchitecture.Application.Employees.Queries;

public class GetAllEmployeesQuery : IRequest<GetAllEmployeesVm>
{
}

public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, GetAllEmployeesVm>
{
    private readonly IDbReadContext _context;

    public GetAllEmployeesHandler(IDbReadContext context)
    {
        _context = context;
    }

    public async Task<GetAllEmployeesVm> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var sql = @"
Select emp_no AS EmpNo,birth_date AS BirthDate,first_name AS FirstName,last_name AS LastName,gender AS gender,hire_date AS HireDate 
from employees";
        var employees = await _context.Connection.QueryAsync<Employee>(sql);

        return new GetAllEmployeesVm
        {
            Employees = employees.ToList()
        };
    }
}

public record GetAllEmployeesVm
{
    public List<Employee> Employees { get; init; }
}