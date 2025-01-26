namespace Infrastructure.ConsoleApp.Tests;

public class KubeServiceTests
{
    [Test]
    public async Task Service_ShouldHavePortsSetup()
    {
        var resources = await Testing.RunAsync<TestStack>();

        var service = resources.OfType<Service>().FirstOrDefault();
        service.ShouldNotBeNull("Service was not found");
        var spec = await service!.Spec.GetValueAsync();
        spec.Ports.ShouldNotBeEmpty();
        spec.Ports.Length.ShouldBe(1);
        spec.Ports[0].Port.ShouldBe(53);
        spec.Ports[0].NodePort.ShouldBe(2656);
    }

    [Test]
    public async Task Deployment_ShouldHaveContainersSetup()
    {
        var resources = await Testing.RunAsync<TestStack>();

        var deployment = resources.OfType<Pulumi.Kubernetes.Apps.V1.Deployment>().FirstOrDefault();
        deployment.ShouldNotBeNull("Deployment was not found");
        var spec = await deployment!.Spec.GetValueAsync();
        spec.Template.Spec.Containers.ShouldNotBeEmpty();
        spec.Template.Spec.Containers.Length.ShouldBe(1);

        var container = spec.Template.Spec.Containers[0];
        container.Image.ShouldBe("TEST-IMAGE");
        container.ImagePullPolicy.ShouldBe("TEST");
        container.Ports.ShouldNotBeEmpty();
        container.Ports.Length.ShouldBe(1);
        container.Ports.ShouldAllBe(x => x.ContainerPortValue == 53);
    }

    private class TestStack : Stack
    {
        public TestStack()
        {
            var test = new KubeService(new LocalService
            {
                Name = "MYTEST",
                Image = "TEST-IMAGE",
                ContainerPort = 53,
                NodePort = 2656,
                ImagePullPolicy = "TEST",
            }, null);
        }
    }
}