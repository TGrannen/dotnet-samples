using Docker = Pulumi.Docker;

namespace Infrastructure.ConsoleApp;

class DemoStack : Stack
{
    public DemoStack()
    {
        // var appImage = new Docker.Image("AppImage", new Docker.ImageArgs
        // {
        //     Build = new Docker.DockerBuild
        //     {
        //         Context = "../",
        //         Dockerfile = "../Infrastructure.BlazorServer/Dockerfile"
        //     },
        //     ImageName = "blazor-server",
        //     SkipPush = true
        // });
        //
        // appImage.ImageName.Apply(s =>
        // {
        //     // new KubeService(new LocalService
        //     // {
        //     //     Image = "blazor-server",
        //     //     Name = "blazor-test",
        //     //     ContainerPort = 80,
        //     //     NodePort = 31111
        //     // });
        //     return "";
        // });

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