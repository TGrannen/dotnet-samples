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
        var sql = @"
INSERT INTO employees (emp_no, birth_date, first_name, last_name, gender, hire_date)
VALUES(@EmpNo,@BirthDate,@FirstName,@LastName, 'M',@HireDate)
RETURNING emp_no";
        var id = await _context.Connection.ExecuteScalarAsync<int>(sql, new
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
            EmployeeNumber = id
        };
    }
}

public record CreateEmployeeCommandVm
{
    public int EmployeeNumber { get; init; }
}