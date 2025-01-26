using KafkaFlowSample.MessageContracts;

namespace KafkaFlowSample.Consumer.Handlers;

public class HelloMessageHandler(ITestScopedService scopedService, ILogger<HelloMessageHandler> logger) : IMessageHandler<HelloMessage>
{
    public Task Handle(IMessageContext context, HelloMessage message)
    {
        logger.LogInformation(
            "Partition: {Partition} | Offset: {Offset} | Message: {Message}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        scopedService.Test();
        return Task.CompletedTask;
    }
}