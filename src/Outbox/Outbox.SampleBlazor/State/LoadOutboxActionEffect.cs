using Amazon.DynamoDBv2.DataModel;
using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.State;

public class LoadOutboxActionEffect : Effect<LoadOutboxAction>
{
    private readonly ILogger<LoadOutboxActionEffect> _logger;
    private readonly IDynamoDBContext _dbContext;

    public LoadOutboxActionEffect(ILogger<LoadOutboxActionEffect> logger, IDynamoDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public override async Task HandleAsync(LoadOutboxAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.Delay != null)
            {
                await Task.Delay(action.Delay.Value);
            }

            var messages = await _dbContext.ScanAsync<OutboxMessage>(new List<ScanCondition>()).GetRemainingAsync();
            dispatcher.Dispatch(new LoadOutboxResultAction { Messages = messages });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error loading outbox");
            dispatcher.Dispatch(new LoadOutboxResultAction { Messages = new List<OutboxMessage>() });
        }
    }
}