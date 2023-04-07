namespace Outbox.Messaging.Abstractions;

public interface IMessage
{
    public Guid Id { get; }
    public string Payload { get; }
}