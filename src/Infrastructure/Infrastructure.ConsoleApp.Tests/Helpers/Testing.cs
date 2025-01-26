using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using Pulumi;
using Pulumi.Testing;

namespace Infrastructure.ConsoleApp.Tests.Helpers;

public static class Testing
{
    public static Task<ImmutableArray<Resource>> RunAsync<T>() where T : Stack, new()
    {
        var mocks = new Mock<IMocks>();
        mocks.Setup(m => m.NewResourceAsync(It.IsAny<MockResourceArgs>()))
            .ReturnsAsync((MockResourceArgs args) => (args.Id ?? "", args.Inputs));
        mocks.Setup(m => m.CallAsync(It.IsAny<MockCallArgs>())).ReturnsAsync((MockCallArgs args) => args.Args);
        return Deployment.TestAsync<T>(mocks.Object, new TestOptions { IsPreview = false });
    }
}