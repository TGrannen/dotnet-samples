namespace KafkaFlowSample.Consumer.Services;

public interface ITestScopedService
{
    void Test();
}

public class TestScopedService(ILogger<TestScopedService> logger) : ITestScopedService
{
    public void Test()
    {
        logger.LogInformation("From my scoped service");
    }
}