namespace FastEndpointsExample.WebApi.Events.OrderCreated;

public class OrderCreatedEvent
{
    public string OrderID { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderTotal { get; set; }
}