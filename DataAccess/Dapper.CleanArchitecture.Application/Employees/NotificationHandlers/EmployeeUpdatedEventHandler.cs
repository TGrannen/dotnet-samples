using Dapper.CleanArchitecture.Domain.Employees.Notifications;
using Microsoft.Extensions.Logging;

namespace Dapper.CleanArchitecture.Application.Employees.NotificationHandlers;

public class EmployeeUpdatedEventHandler : INotificationHandler<EmployeeUpdatedEvent>
{
    private readonly IDbContext _context;
    private readonly ILogger<EmployeeUpdatedEventHandler> _logger;

    public EmployeeUpdatedEventHandler(IDbContext context, ILogger<EmployeeUpdatedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(EmployeeUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling of Employee Updated Notification {@Notification}", notification);

        // This is a nonsense example but it shows that multiple transactions can be made during the lifetime of a single request
        var sql = @"
UPDATE employees SET birth_date = @Now
WHERE emp_no = @EmpNo
RETURNING emp_no";
        var saved = await _context.Connection.ExecuteScalarAsync<int>(sql, new
        {
            EmpNo = notification.EmployeeNumber,
            Now = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}