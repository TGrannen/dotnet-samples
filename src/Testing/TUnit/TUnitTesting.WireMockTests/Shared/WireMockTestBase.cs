using Microsoft.Extensions.Time.Testing;
using WireMock.RequestBuilders;
using WireMock.Server;

namespace TUnitTesting.WireMockTests.Shared;

public class WireMockTestBase : WebApplicationTest<WireMockWebAppFactory, WebApi.Program>
{
    [ClassDataSource<WireMockFixture>(Shared = SharedType.PerTestSession)]
    public WireMockFixture WireMock { get; init; } = null!;

    protected readonly FakeTimeProvider FakeTimeProvider = new();

    protected WireMockServer WireMockServer => WireMock.Server;

    /// <summary> Absolute path under WireMock for a downstream route (leading slash, includes <see cref="IsolationSegment"/>). </summary>
    public string DownstreamPath(string relativePath) => $"/{IsolationSegment}/{relativePath.TrimStart('/')}";

    public string IsolationSegment { get; } = Guid.NewGuid().ToString("N");

    protected override void ConfigureTestConfiguration(IConfigurationBuilder config)
    {
        var wireMockRoot = WireMock.Server.Url!.TrimEnd('/');
        var baseUrl = $"{wireMockRoot}/{IsolationSegment}";

        config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DownstreamCatalog:BaseUrl"] = baseUrl
        });
    }

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(_ => FakeTimeProvider);
    }

    protected IRespondWithAProvider GetEndpoint(string relativePath)
    {
        return WireMockServer.Given(Request.Create().WithPath(DownstreamPath(relativePath)).UsingGet());
    }
}