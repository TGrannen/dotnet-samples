namespace FastEndpointsExample.WebApi.Events.OrderCreated;

public class OrderCreationEmailHandler(ILogger<OrderCreationEmailHandler> logger) : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger _logger = logger;

    public async Task HandleAsync(OrderCreatedEvent eventModel, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        _logger.LogInformation("Order created Email was sent to {Customer}", eventModel.CustomerName);
    }
}