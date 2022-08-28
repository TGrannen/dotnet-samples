namespace SnapshotTesting.Verify.Shared;

public class MoqFixture : IDisposable
{
    public MoqFixture()
    {
        VerifyMoq.Enable();
    }

    public void Dispose()
    {
    }
}

[CollectionDefinition("Moq Collection")]
public class MoqCollection : ICollectionFixture<MoqFixture>
{
}