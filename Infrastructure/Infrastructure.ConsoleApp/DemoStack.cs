namespace Infrastructure.ConsoleApp;

public class DemoStack : Stack
{
    [Output("ip")] public Output<string> IP { get; set; }

    public DemoStack()
    {
        var config = new Config();
        var isMinikube = config.GetBoolean("isMinikube") ?? false;

        var appName = "nginx";

        var appLabels = new InputMap<string>
        {
            { "app", appName },
        };

        var deployment = new Pulumi.Kubernetes.Apps.V1.Deployment(appName, new DeploymentArgs
        {
            Spec = new DeploymentSpecArgs
            {
                Selector = new LabelSelectorArgs
                {
                    MatchLabels = appLabels,
                },
                Replicas = 1,
                Template = new PodTemplateSpecArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Labels = appLabels,
                    },
                    Spec = new PodSpecArgs
                    {
                        Containers =
                        {
                            new ContainerArgs
                            {
                                Name = appName,
                                Image = "nginx",
                                Ports =
                                {
                                    new ContainerPortArgs
                                    {
                                        ContainerPortValue = 80
                                    },
                                },
                            },
                        },
                    },
                },
            },
        });

        var frontend = new Service(appName, new ServiceArgs
        {
            Metadata = new ObjectMetaArgs
            {
                Labels = deployment.Spec.Apply(spec =>
                    spec.Template.Metadata.Labels
                ),
            },
            Spec = new ServiceSpecArgs
            {
                Type = "NodePort",
                Selector = appLabels,
                Ports = new ServicePortArgs
                {
                    Port = 80,
                    TargetPort = 80,
                    Protocol = "TCP",
                    NodePort = 31014
                },
            }
        });

        IP = frontend.Status.Apply(status =>
        {
            var ingress = status.LoadBalancer.Ingress[0];
            return ingress.Ip ?? ingress.Hostname;
        });
    }
}