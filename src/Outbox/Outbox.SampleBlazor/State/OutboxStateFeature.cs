using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.State;

public class OutboxStateFeature : Feature<OutboxState>
{
    public override string GetName() => "Outbox";

    protected override OutboxState GetInitialState()
    {
        return new OutboxState
        {
            Messages = new List<OutboxMessage>()
        };
    }
}