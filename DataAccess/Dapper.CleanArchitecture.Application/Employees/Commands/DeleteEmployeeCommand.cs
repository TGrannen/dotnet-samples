using Dapper.CleanArchitecture.Domain.Employees.Notifications;

namespace Dapper.CleanArchitecture.Application.Employees.Commands;

public record DeleteEmployeeCommand : IRequest
{
    public int EmployeeNumber { get; init; }
}

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IDbContext _context;

    public DeleteEmployeeCommandHandler(IDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var sql = @"
DELETE FROM employees
WHERE emp_no = @EmployeeNumber
RETURNING emp_no";
        var id = await _context.Connection.ExecuteScalarAsync<int>(sql, new { EmployeeNumber = request.EmployeeNumber });
        _context.AddEvent(new EmployeeDeletedEvent { EmployeeNumber = request.EmployeeNumber });
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}