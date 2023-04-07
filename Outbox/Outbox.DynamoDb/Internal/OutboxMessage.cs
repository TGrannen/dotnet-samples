using Outbox.Messaging.Abstractions;

namespace Outbox.DynamoDb.Internal;

[DynamoDBTable("Outbox-Shared")]
internal class OutboxMessage
{
    public Guid Key { get; set; }
    public DateTime Created { get; set; }
    public string? Payload { get; set; }
}