using MassTransit;
using Messaging.RabbitMQ.AdminBlazorContracts.Models;

namespace Messaging.RabbitMQ.Consumer.Consumers;

public class TestMessageConsumer : IConsumer<TestMessage>
{
    readonly ILogger<TestMessageConsumer> _logger;

    public TestMessageConsumer(ILogger<TestMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TestMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received: {@Message}", message);
        await Task.Delay(message.Delay);
        if (message.ToFail)
        {
            _logger.LogWarning("About to throw exception: {@Message}", message);
            throw new Exception("Some bad error");
        }

        _logger.LogInformation("Message processing completed: {@Message}", message);
    }
}