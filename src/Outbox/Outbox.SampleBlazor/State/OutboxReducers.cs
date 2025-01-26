using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.State;

public static class OutboxReducers
{
    [ReducerMethod]
    public static OutboxState LoadOutboxResultActionAction(OutboxState state, LoadOutboxResultAction action)
    {
        return state with
        {
            Messages = action.Messages
        };
    }
}