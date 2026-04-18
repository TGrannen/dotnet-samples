using WireMock.Server;

namespace TUnitTesting.WireMockTests.Shared;

public sealed class WireMockFixture : IAsyncInitializer, IAsyncDisposable
{
    public WireMockServer Server { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Server = WireMockServer.Start();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Server.Stop();
        return ValueTask.CompletedTask;
    }
}
