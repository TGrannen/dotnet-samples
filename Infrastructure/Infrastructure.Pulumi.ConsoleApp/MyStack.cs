using Pulumi.Kubernetes.Core.V1;

class MyStack : Stack
{
    public MyStack()
    {
        var config = new Config();
        var isMinikube = config.GetBoolean("isMinikube") ?? false;

        var appName = "nginx";

        var appLabels = new InputMap<string>{
            { "app", appName },
        };

        var containerArgs = new ContainerArgs
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
                            containerArgs,
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
                    NodePort = 31001
                },
            }
        });

        IP = isMinikube
            ? frontend.Spec.Apply(spec => spec.ClusterIP)
            : frontend.Status.Apply(status =>
            {
                var ingress = status.LoadBalancer.Ingress[0];
                return ingress.Ip ?? ingress.Hostname;
            });
    }

    [Output("ip")]
    public Output<string> IP { get; set; }
}