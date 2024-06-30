namespace AwsMessagingTest.Messages;

public class ChatMessage
{
    public string MessageDescription { get; set; }
}

public class ChatMessageHandler(ILogger<ChatMessageHandler> logger, IOptionsSnapshot<TestingConfig> optionsSnapshot) : IMessageHandler<ChatMessage>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<ChatMessage> messageEnvelope, CancellationToken token = default)
    {
        var config = optionsSnapshot.Value.Chat;

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

        logger.LogInformation("Message Description: {MessageDescription}", message.MessageDescription);

        // Return success so the framework will delete the message from the queue
        return MessageProcessStatus.Success();
    }
}