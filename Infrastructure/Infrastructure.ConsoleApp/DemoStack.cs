namespace Infrastructure.ConsoleApp;

class DemoStack : Stack
{
    public DemoStack()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        IConfiguration config = builder.Build();

        var list = config.GetSection("Services").Get<List<LocalService>>();
        var services = list.Select(service => new KubeService(service)).ToList();
        UrlAddresses = Output.All(services.Select(x => x.UrlAddress));
    }

    [Output] public Output<ImmutableArray<string>> UrlAddresses { get; set; }
}