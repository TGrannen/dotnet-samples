namespace AwsMessagingTest.Messages;

public class ChatMessage
{
    public string MessageDescription { get; set; }
}

public class ChatMessageHandler(ILogger<ChatMessageHandler> logger, IOptionsSnapshot<TestingConfig> optionsSnapshot) : IMessageHandler<ChatMessage>
{
    public Task<MessageProcessStatus> HandleAsync(MessageEnvelope<ChatMessage> messageEnvelope, CancellationToken token = default)
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

        var config = optionsSnapshot.Value.Chat;
        if (config.Throw)
        {
            throw new Exception($"Dummy test{DateTime.Now}");
        }

        if (config.ReturnFailure)
        {
            return Task.FromResult(MessageProcessStatus.Failed());
        }

        var message = messageEnvelope.Message;

        logger.LogInformation("Message Description: {MessageDescription}", message.MessageDescription);

        // Return success so the framework will delete the message from the queue
        return Task.FromResult(MessageProcessStatus.Success());
    }
}