namespace TUnitTesting.IntegrationTests.IntegrationTests.WeatherForecast;

[Category("Integration")]
public class IntegrationTests : WebApplicationTest<WeatherForecastWebAppFactory, WebApi.Program>
{
    [Test]
    public async Task Test1()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/weatherforecast");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
