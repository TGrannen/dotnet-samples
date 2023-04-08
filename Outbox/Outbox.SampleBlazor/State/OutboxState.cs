using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.State;

public record OutboxState
{
    public List<OutboxMessage> Messages { get; init; } = new();
}