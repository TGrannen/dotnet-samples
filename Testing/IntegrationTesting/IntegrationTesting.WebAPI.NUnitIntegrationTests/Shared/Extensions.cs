namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.Shared;

public static class Extensions
{
    public static T CreateRefitClient<TFactory, T>(this WebApplicationFactory<TFactory> factory) where TFactory : class
    {
        return RestService.For<T>(factory.CreateClient());
    }
}