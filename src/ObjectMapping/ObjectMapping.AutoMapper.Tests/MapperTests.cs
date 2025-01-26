using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ObjectMapping.AutoMapper.Models;
using Xunit;

namespace ObjectMapping.AutoMapper.Tests;

public class MapperTests
{
    private readonly IMapper _mapper;

    public MapperTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAutoMapper(typeof(User));
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void ConfigurationProvider_VerifyConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}