using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mocking.Mediator.Tests.Fixtures;
using Mocking.Moq.Extensions;
using Moq;
using Xunit;

namespace Mocking.Mediator.Tests;

public class MediatRIntegrationsTests
{
    private readonly ApplicationFixture _fixture;

    public MediatRIntegrationsTests()
    {
        _fixture = new ApplicationFixture();
    }

    [Fact]
    public async Task GetAllOrdersQuery_ShouldLogMessage_WhenDataIsReturned()
    {
        var query = new GetAllOrdersQuery();

        var result = await _fixture.SendAsync(query);

        result.Should().NotBeNullOrEmpty();

        _fixture
            .GetMockedLogger<GetAllOrdersQueryHandler>()
            .VerifyLogging(LogLevel.Information, $"Got list of {result.Count} Entries", Times.Once);
    }

    [Fact]
    public async Task GetAllOrdersQuery_ShouldNotHaveAnyWarningsOrErrorsLogged_WhenDataIsReturned()
    {
        var query = new GetAllOrdersQuery();

        var result = await _fixture.SendAsync(query);

        result.Should().NotBeNullOrEmpty();

        _fixture
            .GetMockedLogger<GetAllOrdersQueryHandler>()
            .VerifyLogging(LogLevel.Warning, Times.Never)
            .VerifyLogging(LogLevel.Error, Times.Never)
            .VerifyLogging(LogLevel.Critical, Times.Never)
            ;
    }
}