using Dapper.CleanArchitecture.Domain.Employees.Notifications;
using Microsoft.Extensions.Logging;

namespace Dapper.CleanArchitecture.Application.Employees.NotificationHandlers;

public class DeleteEmployeeNotificationHandler : INotificationHandler<EmployeeDeletedEvent>
{
    private readonly ILogger<DeleteEmployeeNotificationHandler> _logger;

    public DeleteEmployeeNotificationHandler(ILogger<DeleteEmployeeNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EmployeeDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling of deleted Notification {@Notification}", notification);

        return Task.CompletedTask;
    }
}