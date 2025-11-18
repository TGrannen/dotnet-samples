using AutoMapper;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectMapping.AutoMapper.Profiles;
using Xunit;

namespace ObjectMapping.AutoMapper.Tests;

public class MapperTests
{
    private readonly IMapper _mapper;
    private readonly ILoggerFactory _loggerFactory = A.Fake<ILoggerFactory>();

    public MapperTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAutoMapper(x => x.AddProfile<UserProfile>());
        services.AddSingleton(_loggerFactory);
        A.CallTo(() => _loggerFactory.CreateLogger(A<string>.Ignored)).Returns(A.Fake<ILogger>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void ConfigurationProvider_VerifyConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}