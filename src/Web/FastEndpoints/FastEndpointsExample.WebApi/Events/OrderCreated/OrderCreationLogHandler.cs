namespace FastEndpointsExample.WebApi.Events.OrderCreated;

public class OrderCreationLogHandler(ILogger<OrderCreationLogHandler> logger) : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger _logger = logger;

    public async Task HandleAsync(OrderCreatedEvent eventModel, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        _logger.LogInformation("Order created event received:[{OrderID}]", eventModel.OrderID);
    }
}