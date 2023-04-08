namespace Outbox.SampleBlazor.Services;

public class MessagePublisherSettings
{
    public bool ShouldThrowException { get; set; } = false;
    public int PublishDelay { get; set; } = 1500;
}