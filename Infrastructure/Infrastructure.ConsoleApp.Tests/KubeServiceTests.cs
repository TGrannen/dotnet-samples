namespace Infrastructure.ConsoleApp.Tests;

public class KubeServiceTests
{
    [Test]
    public async Task Service_ShouldHavePortsSetup()
    {
        var resources = await Testing.RunAsync<TestStack>();

        var service = resources.OfType<Service>().FirstOrDefault();
        service.Should().NotBeNull("Service was not found");
        var spec = await service!.Spec.GetValueAsync();
        spec.Ports.Should().NotBeEmpty().And.HaveCount(1);
        spec.Ports[0].Port.Should().Be(53);
        spec.Ports[0].NodePort.Should().Be(2656);
    }

    [Test]
    public async Task Deployment_ShouldHaveContainersSetup()
    {
        var resources = await Testing.RunAsync<TestStack>();

        var deployment = resources.OfType<Pulumi.Kubernetes.Apps.V1.Deployment>().FirstOrDefault();
        deployment.Should().NotBeNull("Deployment was not found");
        var spec = await deployment!.Spec.GetValueAsync();
        spec.Template.Spec.Containers.Should().NotBeNullOrEmpty().And.HaveCount(1);

        var container = spec.Template.Spec.Containers[0];
        container.Image.Should().Be("TEST-IMAGE");
        container.ImagePullPolicy.Should().Be("TEST");
        container.Ports.Should().NotBeNullOrEmpty().And.HaveCount(1).And.Satisfy(x => x.ContainerPortValue == 53);
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