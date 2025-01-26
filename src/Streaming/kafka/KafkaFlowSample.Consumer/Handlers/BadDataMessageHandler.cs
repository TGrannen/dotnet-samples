using KafkaFlowSample.Consumer.Configuration;
using KafkaFlowSample.MessageContracts;
using Microsoft.Extensions.Options;

namespace KafkaFlowSample.Consumer.Handlers;

public class BadDataMessageHandler(ILogger<BadDataMessageHandler> logger, IOptionsMonitor<TestingConfig> optionsSnapshot)
    : IMessageHandler<BadDataMessage>
{
    public Task Handle(IMessageContext context, BadDataMessage message)
    {
        logger.LogInformation("Received bad data message {@Message}", message);

        if (!message.ThrowException)
        {
            return Task.CompletedTask;
        }

        if (optionsSnapshot.CurrentValue.ActuallyThrowExceptions)
        {
            throw new Exception("We got some bad data");
        }

        logger.LogWarning("Not throwing exceptions due to config {@Message}", message);
        return Task.CompletedTask;
    }
}