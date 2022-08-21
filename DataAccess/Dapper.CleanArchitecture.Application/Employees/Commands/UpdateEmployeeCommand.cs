namespace Dapper.CleanArchitecture.Application.Employees.Commands;

public record UpdateEmployeeCommand : IRequest<UpdateEmployeeCommandVm>
{
    public int EmployeeNumber { get; init; }
    public DateTime BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, UpdateEmployeeCommandVm>
{
    private readonly IDbContext _context;

    public UpdateEmployeeCommandHandler(IDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateEmployeeCommandVm> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var sql = @"
UPDATE employees SET 
     birth_date = @BirthDate,
     first_name = @FirstName,
     last_name = @LastName,
     hire_date = @HireDate
WHERE emp_no = @EmpNo
RETURNING emp_no";
        var saved = await _context.Connection.ExecuteScalarAsync<int>(sql, new
        {
            EmpNo = request.EmployeeNumber,
            BirthDate = request.BirthDate,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HireDate = request.HireDate,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return new UpdateEmployeeCommandVm { Success = saved == request.EmployeeNumber };
    }
}

public record UpdateEmployeeCommandVm
{
    public bool Success { get; init; }
}