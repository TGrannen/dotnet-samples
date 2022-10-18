using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject1IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

public class AppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public T CreateRefitClient<T>()
    {
        return RestService.For<T>(CreateClient());
    }
}