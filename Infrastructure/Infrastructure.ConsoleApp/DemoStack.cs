using System.Collections.Immutable;
using Infrastructure.ConsoleApp.Models;
using Infrastructure.ConsoleApp.Services;

namespace Infrastructure.ConsoleApp;

class DemoStack : Stack
{
    public DemoStack()
    {
        var list = new List<LocalService>
        {
            new() { AppName = "nginx-1", Image = "nginx", NodePort = 32001 },
            new() { AppName = "nginx-2", Image = "nginx", NodePort = 32002 },
            new() { AppName = "nginx-3", Image = "nginx", NodePort = 32003 },
        };
        var services = list.Select(service => new KubeService(service)).ToList();

        IpAddresses = Output.All(services.Select(x => x.IP));
        Ports = Output.All(services.Select(x => x.NodePort));
    }

    [Output] public Output<ImmutableArray<string>> IpAddresses { get; set; }
    [Output] public Output<ImmutableArray<int>> Ports { get; set; }
}