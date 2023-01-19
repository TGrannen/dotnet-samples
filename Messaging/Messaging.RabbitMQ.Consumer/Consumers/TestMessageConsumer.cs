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
        _logger.LogInformation("Message received: {@Message}", context.Message);

        await Task.Delay(2000);

        _logger.LogInformation("Message processing completed: {@Message}", context.Message);
    }
}