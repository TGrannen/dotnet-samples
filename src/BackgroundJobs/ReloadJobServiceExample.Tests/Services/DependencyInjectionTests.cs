using ReloadJobServiceExample.Services;
using ReloadJobServiceExample.Services.Jobs;

namespace ReloadJobServiceExample.Tests.Services;

public class DependencyInjectionTests
{
    [Fact]
    public void AddJobs_ShouldRegister_Job1()
    {
        var provider = BuildServiceProviderWithAddJobs();
        var job1 = provider.GetService<ReloadJob1>();
        job1.ShouldNotBeNull();
    }

    [Fact]
    public void AddJobs_ShouldRegister_Job2()
    {
        var provider = BuildServiceProviderWithAddJobs();
        var job2 = provider.GetService<ReloadJob2>();
        job2.ShouldNotBeNull();
    }

    [Fact]
    public void AddJobs_ShouldRegister_ReloadJobService()
    {
        var provider = BuildServiceProviderWithAddJobs();
        var service = provider.GetService<ReloadJobService<ReloadJob1>>();
        service.ShouldNotBeNull();
    }


    private ServiceProvider BuildServiceProviderWithAddJobs()
    {
        var collection = new ServiceCollection();
        collection.AddLogging();
        collection.AddJobs<DependencyInjectionTests>();
        var provider = collection.BuildServiceProvider();
        return provider;
    }
}

public class ReloadJob1 : IReloadJob
{
    public Task<bool> Execute(CancellationToken token)
    {
        return Task.FromResult(true);
    }
}

public class ReloadJob2 : IReloadJob
{
    public Task<bool> Execute(CancellationToken token)
    {
        return Task.FromResult(true);
    }
}