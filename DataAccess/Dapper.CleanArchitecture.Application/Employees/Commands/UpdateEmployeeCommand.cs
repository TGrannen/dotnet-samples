using Dapper.CleanArchitecture.Domain.Employees.Notifications;

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
        var employeeNumber = await _context.Connection.ExecuteScalarAsync<int>(UpdateSql, new
        {
            EmpNo = request.EmployeeNumber,
            BirthDate = request.BirthDate,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HireDate = request.HireDate,
        });
        _context.AddEvent(new EmployeeUpdatedEvent { EmployeeNumber = request.EmployeeNumber });
        await _context.SaveChangesAsync(cancellationToken);
        return new UpdateEmployeeCommandVm { Success = employeeNumber == request.EmployeeNumber };
    }

    private const string UpdateSql = @"
UPDATE employees SET 
     birth_date = @BirthDate,
     first_name = @FirstName,
     last_name = @LastName,
     hire_date = @HireDate
WHERE emp_no = @EmpNo
RETURNING emp_no";
}

public record UpdateEmployeeCommandVm
{
    public bool Success { get; init; }
}