using Dapper.CleanArchitecture.Domain.Employees;

namespace Dapper.CleanArchitecture.Application.Employees.Queries;

public class GetAllEmployeesQuery : IRequest<List<Employee>>
{
}

public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, List<Employee>>
{
    private readonly IDbReadContext _context;

    public GetAllEmployeesHandler(IDbReadContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _context.Connection.QueryAsync<Employee>(QuerySql);
        return employees.ToList();
    }
    
    private const string QuerySql = @"
Select emp_no AS EmpNo,birth_date AS BirthDate,first_name AS FirstName,last_name AS LastName,gender AS gender,hire_date AS HireDate 
from employees
order by emp_no";
}