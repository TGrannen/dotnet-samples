using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.DILibrary.DIContainers;

public class NetCoreDISetup
{
    public NetCoreDISetup()
    {
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>();
        Provider = services.BuildServiceProvider();
    }

    public IServiceProvider Provider { get; set; }
}