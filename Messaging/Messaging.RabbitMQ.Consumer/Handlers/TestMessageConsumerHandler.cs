using MassTransit;
using Messaging.RabbitMQ.AdminBlazorContracts.Models;
using Messaging.RabbitMQ.Consumer.Consumers;
using Wolverine.Attributes;

namespace Messaging.RabbitMQ.Consumer.Handlers;

public static class TestMessageConsumerHandler
{
    //https://wolverine.netlify.app/guide/handlers/error-handling.html#scoping

    // [ScheduleRetry(typeof(IOException), 5)]
    // [RequeueOn(typeof(InvalidOperationException))]
     // [MaximumAttempts(1)]
    // [MoveToErrorQueueOn(typeof(Exception))]
    // [RetryNow(typeof(Exception), 50, 100, 250)]
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
