namespace Outbox.DynamoDb.Internal;

[DynamoDBTable("Outbox-Shared")]
public class OutboxMessage
{
    public Guid Key { get; init; }
    public DateTime Created { get; init; }
    public string Payload { get; init; } = string.Empty;
}