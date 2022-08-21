using Dapper.CleanArchitecture.Web.Services;
using DotNet.Testcontainers.Containers;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared.Fixtures;

public class DummyContainerService : IContainerService
{
    public PostgreSqlTestcontainer Container { get; set; }
    public Task RunContainer()
    {
        return Task.CompletedTask;
    }

    public Task StopContainer()
    {
        return Task.CompletedTask;
    }
}