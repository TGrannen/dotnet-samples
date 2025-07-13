namespace TUnitTesting.Tests.IntegrationTests;

public class IntegrationTests
{
    [ClassDataSource<WeatherForecastWebAppFactory>(Shared = SharedType.PerTestSession)]
    public required WeatherForecastWebAppFactory WeatherForecastWebAppFactory { get; init; }

    [Test]
    public async Task Test1()
    {
        var client = WeatherForecastWebAppFactory.CreateClient();

        var response = await client.GetAsync("/weatherforecast");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}