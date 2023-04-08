using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.State;

public class LoadOutboxAction
{
    public TimeSpan? Delay { get; init; }
}

public class LoadOutboxResultAction
{
    public List<OutboxMessage> Messages { get; set; }
}