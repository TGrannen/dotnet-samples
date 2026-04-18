using Microsoft.AspNetCore.Hosting;

namespace TUnitTesting.WireMockTests.Shared;

public class WireMockWebAppFactory : TestWebApplicationFactory<WebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}