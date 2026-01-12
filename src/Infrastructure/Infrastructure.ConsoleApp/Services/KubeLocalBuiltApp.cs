using Pulumi.Docker.Inputs;

namespace Infrastructure.ConsoleApp.Services;

using Docker = Pulumi.Docker;

public class KubeLocalBuiltApp
{
    [Output("ip")] public Output<string> UrlAddress { get; set; }
    [Output] public Output<string> Image { get; set; }
    [Output] public Output<string> Repo { get; set; }

    public KubeLocalBuiltApp(LocalApp app)
    {
        var appImage = new Docker.Image(app.Name, new Docker.ImageArgs
        {
            Build = new DockerBuildArgs
            {
                Context = app.Context,
                Dockerfile = app.Dockerfile,
            },
            ImageName = app.ImageName,
            SkipPush = true
        });
        Image = appImage.ImageName;
        Repo = appImage.RegistryServer;

        var service = new KubeService(new LocalService
        {
            Name = app.Name,
            Image = $"{app.ImageName}:latest",
            NodePort = app.NodePort,
            ImagePullPolicy = "Never"
        }, new CustomResourceOptions { DependsOn = appImage });
        UrlAddress = service.UrlAddress;
    }
}