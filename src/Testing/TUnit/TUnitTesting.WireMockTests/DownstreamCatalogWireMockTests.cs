using System.Net.Http.Json;
using TUnitTesting.WebApi.Clients;

namespace TUnitTesting.WireMockTests;

[Category("WireMock")]
public class DownstreamCatalogWireMockTests : WireMockTestBase
{
    [Test]
    public async Task GetItem_returns_body_when_downstream_succeeds()
    {
        GetEndpoint("items/1")
            .InScenario(IsolationSegment)
            .RespondWithJson(new CatalogItemResponse { Id = 1, Name = "alpha" });

        var client = Factory.CreateClient();
        var response = await client.GetFromJsonAsync<CatalogItemResponse>("/api/catalog/items/1");

        await Assert.That(response).IsNotNull();
        await Assert.That(response!.Id).IsEqualTo(1);
        await Assert.That(response.Name).IsEqualTo("alpha");
    }

    [Test]
    public async Task GetItem_retries_on_503_when_fake_time_advances()
    {
        // Scenario state is server-wide; must be unique when tests run in parallel on one WireMock instance.
        GetEndpoint("items/42")
            .InScenario(IsolationSegment)
            .WillSetStateTo("failedOnce")
            .RespondWithStatusCode(HttpStatusCode.ServiceUnavailable);

        GetEndpoint("items/42")
            .InScenario(IsolationSegment)
            .WhenStateIs("failedOnce")
            .RespondWithJson(new CatalogItemResponse { Id = 42, Name = "after retry" });

        var client = Factory.CreateClient();
        var requestTask = client.GetFromJsonAsync<CatalogItemResponse>("/api/catalog/items/42");

        while (!requestTask.IsCompleted)
        {
            await Task.Delay(20);
            FakeTimeProvider.Advance(TimeSpan.FromSeconds(30));
        }

        var response = await requestTask.ConfigureAwait(true);

        await Assert.That(response).IsNotNull();
        await Assert.That(response!.Id).IsEqualTo(42);
        await Assert.That(response.Name).IsEqualTo("after retry");
    }
}