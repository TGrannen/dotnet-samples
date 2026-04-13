using System.Reflection;

namespace ApiService.Api.Tests.Architecture;

public class MediatRArchitectureTests
{
    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    [Test]
    [Arguments("MediatR.ISender", nameof(ISender))]
    [Arguments("MediatR.IMediator", nameof(IMediator))]
    [Arguments("MediatR.IPublisher", nameof(IPublisher))]
    public async Task IRequestHandlers_Should_NotDependOn_MediatorSendAbstractions(
        string mediatrDependencyTypeName,
        string forbiddenDependencyDisplayName)
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .ShouldNot()
            .HaveDependencyOn(mediatrDependencyTypeName)
            .GetResult();

        var failing = string.Join(", ", result.FailingTypes?.Select(t => t.FullName)?? []);
        await Assert.That(failing)
            .IsEqualTo(string.Empty)
            .Because($"Handlers must not depend on {forbiddenDependencyDisplayName}");
    }
}
