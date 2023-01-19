using MassTransit;

namespace Messaging.RabbitMQ.Consumer.Consumers;

public record OrderMessage(Guid OrderId);

public class OrderMessageConsumer : IConsumer<OrderMessage>
{
    readonly ILogger<OrderMessageConsumer> _logger;

    public OrderMessageConsumer(ILogger<OrderMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderMessage> context)
    {
        _logger.LogInformation("Message received: {OrderId}", context.Message.OrderId);

        await Task.Delay(2000);

        _logger.LogInformation("Order Completed: {OrderId}", context.Message.OrderId);
    }
}