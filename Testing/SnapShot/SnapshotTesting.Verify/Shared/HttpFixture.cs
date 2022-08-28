namespace SnapshotTesting.Verify.Shared;

public class HttpFixture : IDisposable
{
    public HttpFixture()
    {
        VerifyHttp.Enable();
    }

    public void Dispose()
    {
    }
}

[CollectionDefinition("Http Collection")]
public class HttpCollection : ICollectionFixture<HttpFixture>
{
}