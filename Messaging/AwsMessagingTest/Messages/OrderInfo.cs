namespace AwsMessagingTest.Messages;

public class OrderInfo
{
    public int Id { get; set; }
    public string Value { get; set; }
}

public class OrderInfoHandler(ILogger<OrderInfoHandler> logger, IOptionsSnapshot<TestingConfig> optionsSnapshot) : IMessageHandler<OrderInfo>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<OrderInfo> messageEnvelope, CancellationToken token = new())
    {
        var config = optionsSnapshot.Value.Order;

        await Task.Delay(config.Delay, token);

        if (config.Throw)
        {
            throw new Exception($"Dummy test{DateTime.Now}");
        }

        if (config.ReturnFailure)
        {
            return MessageProcessStatus.Failed();
        }

        var message = messageEnvelope.Message;

        logger.LogInformation("Order details: {@Data}", message);

        // Return success so the framework will delete the message from the queue
        return MessageProcessStatus.Success();
    }
}