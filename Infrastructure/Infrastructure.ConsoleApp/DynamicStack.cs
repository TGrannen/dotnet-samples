namespace Infrastructure.ConsoleApp;

public class DynamicStack : Stack
{
    [Output] public Output<ImmutableArray<string>> UrlAddresses { get; set; }

    public DynamicStack()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        IConfiguration config = builder.Build();

        var list = config.GetSection("Services").Get<List<LocalService>>();
        var services = list.Select(service => new KubeService(service)).ToList();

        var appList = config.GetSection("LocalApps").Get<List<LocalApp>>();
        var apps = appList.Select(service => new KubeLocalBuiltApp(service)).ToList();

        var inputs = apps.Select(x => x.UrlAddress).ToList();
        inputs.AddRange(services.Select(x => x.UrlAddress));
        UrlAddresses = Output.All(inputs);
    }
}