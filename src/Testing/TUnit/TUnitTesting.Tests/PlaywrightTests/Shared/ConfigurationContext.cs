using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace TUnitTesting.Tests.PlaywrightTests.Shared;

public class ConfigurationContext
{
    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;

    public ConfigurationContext()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<ConfigurationContext>()
            .AddEnvironmentVariables()
            .Build();
        var services = new ServiceCollection();
        services.Configure<AuthOptions>(_configuration.GetSection(AuthOptions.SectionName));
        _serviceProvider = services.BuildServiceProvider();
    }

    public IConfiguration Configuration => _configuration;

    public T GetOptions<T>()
    {
        return _serviceProvider.GetRequiredService<IOptionsMonitor<T>>().CurrentValue;
    }
}