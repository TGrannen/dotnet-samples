﻿namespace Infrastructure.ConsoleApp.Services;

public class KubeService
{
    [Output("ip")] public Output<string> UrlAddress { get; set; }

    public KubeService(LocalService service, CustomResourceOptions customResourceOptions = null)
    {
        var appLabels = new InputMap<string>
        {
            { "app", service.Name },
        };

        var containerArgs = new ContainerArgs
        {
            Name = service.Name,
            Image = service.Image,
            ImagePullPolicy = service.ImagePullPolicy,
            Ports =
            {
                new ContainerPortArgs
                {
                    ContainerPortValue = service.ContainerPort
                },
            }
        };

        var deployment = new Pulumi.Kubernetes.Apps.V1.Deployment(service.Name, new DeploymentArgs
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
        }, customResourceOptions);

        var frontend = new Service(service.Name, new ServiceArgs
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

        UrlAddress = frontend.Status.Apply(status =>
        {
            var ingress = status?.LoadBalancer?.Ingress[0];
            return $"{service.Name}:  {ingress?.Ip ?? ingress?.Hostname}:{service.NodePort}";
        });
    }
}