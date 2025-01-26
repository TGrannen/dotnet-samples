using Dapper.CleanArchitecture.Domain.Employees;

namespace Dapper.CleanArchitecture.Application.Employees.Queries;

public class GetEmployeeByIdQuery : IRequest<Employee>
{
    public int EmployeeNumber { get; set; }
}

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Employee>
{
    private readonly IDbReadContext _context;

    public GetEmployeeByIdQueryHandler(IDbReadContext context)
    {
        _context = context;
    }

    public async Task<Employee> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Connection.QueryFirstOrDefaultAsync<Employee>(QuerySql, new
        {
            EmployeeNumber = request.EmployeeNumber
        });
    }

    private const string QuerySql = @"
     Select emp_no AS EmpNo,birth_date AS BirthDate,first_name AS FirstName,last_name AS LastName,gender AS gender,hire_date AS HireDate 
     from employees
     WHERE emp_no = @EmployeeNumber";
}