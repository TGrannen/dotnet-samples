namespace Dapper.CleanArchitecture.Application.Employees.Commands;

public record CreateEmployeeCommand : IRequest<CreateEmployeeCommandVm>
{
    public int EmployeeNumber { get; init; }
    public DateTime BirthDate { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime HireDate { get; init; }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CreateEmployeeCommandVm>
{
    private readonly IDbContext _context;

    public CreateEmployeeCommandHandler(IDbContext context)
    {
        _context = context;
    }

    public async Task<CreateEmployeeCommandVm> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeNumber = await _context.Connection.ExecuteScalarAsync<int>(CreateSql, new
        {
            EmpNo = request.EmployeeNumber,
            BirthDate = request.BirthDate,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HireDate = request.HireDate,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return new CreateEmployeeCommandVm
        {
            EmployeeNumber = employeeNumber
        };
    }

    private const string CreateSql = @"
INSERT INTO employees (emp_no, birth_date, first_name, last_name, gender, hire_date)
VALUES(@EmpNo,@BirthDate,@FirstName,@LastName, 'M',@HireDate)
RETURNING emp_no";
}

public record CreateEmployeeCommandVm
{
    public int EmployeeNumber { get; init; }
}