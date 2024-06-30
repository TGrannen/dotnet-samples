namespace AwsMessagingTest.Config;

public class TestingConfig
{
    public TimeSpan RateOfChat { get; set; }
    public TimeSpan BackoffDelay { get; set; }
    public QueueHandlerConfig Chat { get; set; }
    public QueueHandlerConfig Order { get; set; }
}

public class QueueHandlerConfig
{
    public TimeSpan Delay { get; set; }
    public bool Throw { get; set; }
    public bool ReturnFailure { get; set; }
}