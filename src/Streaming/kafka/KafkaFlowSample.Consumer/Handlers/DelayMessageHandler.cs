using KafkaFlowSample.MessageContracts;

namespace KafkaFlowSample.Consumer.Handlers;

public class DelayMessageHandler(ILogger<DelayMessageHandler> logger) : IMessageHandler<DelayMessage>
{
    public async Task Handle(IMessageContext context, DelayMessage message)
    {
        logger.LogInformation("Received Delay message {@Message}", message);

        await Task.Delay(message.Delay, context.ConsumerContext.WorkerStopped);
        
        logger.LogInformation("Delay complete {@Message}", message);
    }
}