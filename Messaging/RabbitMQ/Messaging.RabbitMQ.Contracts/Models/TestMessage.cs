namespace Messaging.RabbitMQ.AdminBlazorContracts.Models;

public record TestMessage
{
    public Guid MyId { get; init; }
    public DateTime Time { get; init; }
}