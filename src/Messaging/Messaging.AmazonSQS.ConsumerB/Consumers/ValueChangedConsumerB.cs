using MassTransit;
using Messaging.Events.Contracts;
using Microsoft.Extensions.Logging;

namespace Messaging.AmazonSQS.ConsumerB.Consumers;

public class ValueChangedConsumerB : IConsumer<IValueChanged>
{
    private readonly ILogger<ValueChangedConsumerB> _logger;

    public ValueChangedConsumerB(ILogger<ValueChangedConsumerB> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IValueChanged> context)
    {
        _logger.LogInformation("Consumed B message: {@Message}", context.Message);
        _logger.LogInformation("Full Detail B message: {@Context}", context);
        return Task.CompletedTask;
    }
}