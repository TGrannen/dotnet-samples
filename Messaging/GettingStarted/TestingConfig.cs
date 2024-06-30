namespace GettingStarted;

public class TestingConfig
{
    public TimeSpan RateOfMessage { get; set; }
    public TimeSpan BackoffDelay { get; set; }
    public QueueHandlerConfig Hello { get; set; }
}

public class QueueHandlerConfig
{
    public TimeSpan Delay { get; set; }
    public bool Throw { get; set; }
}