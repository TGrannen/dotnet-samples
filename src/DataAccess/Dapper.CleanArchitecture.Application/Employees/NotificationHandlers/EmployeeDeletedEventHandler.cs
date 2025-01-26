using Dapper.CleanArchitecture.Domain.Employees.Notifications;
using Microsoft.Extensions.Logging;

namespace Dapper.CleanArchitecture.Application.Employees.NotificationHandlers;

public class EmployeeDeletedEventHandler : INotificationHandler<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventHandler> _logger;

    public EmployeeDeletedEventHandler(ILogger<EmployeeDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EmployeeDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling of deleted Notification {@Notification}", notification);

        return Task.CompletedTask;
    }
}