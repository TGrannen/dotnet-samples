using FastEndpointsExample.WebApi.Events.OrderCreated;

namespace FastEndpointsExample.WebApi.Endpoints;

[HttpPost("/api/sales/orders/create")]
[AllowAnonymous]
public class CreateOrderEndpoint : Endpoint<CreateOrderEndpoint.Request>
{
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await PublishAsync(new OrderCreatedEvent
        {
            OrderID = Guid.NewGuid().ToString(),
            CustomerName = req.Customer,
            OrderTotal = req.OrderValue
        }, cancellation: ct);

        await SendOkAsync(ct);
    }

    public class Request
    {
        public required string Customer { get; set; }
        public required decimal OrderValue { get; set; }
    }
}