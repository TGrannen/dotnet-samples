namespace SnapshotTesting.Verify.Shared;

public class LoggingFixture : IDisposable
{
    public LoggingFixture()
    {
        VerifyMicrosoftLogging.Enable();
    }

    public void Dispose()
    {
    }
}