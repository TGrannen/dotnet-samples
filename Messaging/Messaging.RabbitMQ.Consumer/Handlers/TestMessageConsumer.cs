using MassTransit;
using Messaging.RabbitMQ.AdminBlazorContracts.Models;
using Messaging.RabbitMQ.Consumer.Consumers;

namespace Messaging.RabbitMQ.Consumer.Handlers;

public static class TestMessageConsumerHandler
{
    public static async ValueTask Handle(TestMessage message, ILogger<TestMessageConsumer> logger)
    {
        logger.LogInformation("Wolverine Message received: {@Message}", message);
        await Task.Delay(message.Delay);
        if (message.ToFail)
        {
            logger.LogWarning("Wolverine About to throw exception: {@Message}", message);
            throw new Exception("Some bad error");
        }

        logger.LogInformation("Wolverine Message processing completed: {@Message}", message);
    }
}
