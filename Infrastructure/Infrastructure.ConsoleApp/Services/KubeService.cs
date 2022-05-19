using System.Collections.Immutable;
using Infrastructure.ConsoleApp.Models;
using Pulumi.Kubernetes.Types.Outputs.Core.V1;
using Service = Pulumi.Kubernetes.Core.V1.Service;

namespace Infrastructure.ConsoleApp.Services;

public class KubeService
{
    public KubeService(LocalService service)
    {
        var appLabels = new InputMap<string>
        {
            { "app", service.AppName },
        };

        var containerArgs = new ContainerArgs
        {
            Name = service.AppName,
            Image = service.Image,
            Ports =
            {
                new ContainerPortArgs
                {
                    ContainerPortValue = service.ContainerPort
                },
            },
        };

        var deployment = new Pulumi.Kubernetes.Apps.V1.Deployment(service.AppName, new DeploymentArgs
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

        var frontend = new Service(service.AppName, new ServiceArgs
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
                    Port = service.ContainerPort,
                    TargetPort = service.ContainerPort,
                    Protocol = "TCP",
                    NodePort = service.NodePort
                },
            }
        });

        IP = frontend.Status.Apply(status =>
        {
            var ingress = status.LoadBalancer.Ingress[0];
            return ingress.Ip ?? ingress.Hostname;
        });
        NodePort = frontend.Status.Apply(_ => service.NodePort);
    }

    [Output("ip")] public Output<string> IP { get; set; }
    [Output("port")] public Output<int> NodePort { get; set; }
}