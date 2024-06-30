namespace AwsMessagingTest.Messages;

public class OrderInfo
{
    public int Id { get; set; }
    public string Value { get; set; }
}

public class OrderInfoHandler(ILogger<OrderInfoHandler> logger, IOptionsSnapshot<TestingConfig> optionsSnapshot) : IMessageHandler<OrderInfo>
{
    public Task<MessageProcessStatus> HandleAsync(MessageEnvelope<OrderInfo> messageEnvelope, CancellationToken token = new())
    {
        // Add business and validation logic here
        if (messageEnvelope == null)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        if (messageEnvelope.Message == null)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        var config = optionsSnapshot.Value.Order;
        if (config.Throw)
        {
            throw new Exception($"Order Dummy test{DateTime.Now}");
        }

        if (config.ReturnFailure)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        var message = messageEnvelope.Message;

        logger.LogInformation("Order details: {@Data}", message);

        // Return success so the framework will delete the message from the queue
        return Task.FromResult(MessageProcessStatus.Success());
    }
}