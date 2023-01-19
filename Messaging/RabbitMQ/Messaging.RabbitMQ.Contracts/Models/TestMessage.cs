namespace Messaging.RabbitMQ.AdminBlazorContracts.Models;

public record TestMessage
{
    public Guid MyId { get; init; }
    public DateTime Time { get; init; }
    public bool ToFail { get; init; }
    public TimeSpan Delay { get; init; } = TimeSpan.FromSeconds(2);
}