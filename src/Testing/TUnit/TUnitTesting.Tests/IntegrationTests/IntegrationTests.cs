namespace TUnitTesting.Tests.IntegrationTests;

public class IntegrationTests
{
    [ClassDataSource<WebAppFactory>(Shared = SharedType.PerTestSession)]
    public required WebAppFactory WebAppFactory { get; init; }

    [Test]
    public async Task Test1()
    {
        var client = WebAppFactory.CreateClient();

        var response = await client.GetAsync("/weatherforecast");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}