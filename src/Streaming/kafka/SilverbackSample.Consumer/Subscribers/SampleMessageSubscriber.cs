namespace SilverbackSample.Consumer.Subscribers;

public class SampleMessageSubscriber(ILogger<SampleMessageSubscriber> logger)
{
    public void OnMessageReceived(SampleMessage message)
    {
        logger.LogInformation("Received {@Message}", message);
    }
}