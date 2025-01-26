namespace Outbox.Messaging.Abstractions;

public class Message
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Payload { get; init; }
}