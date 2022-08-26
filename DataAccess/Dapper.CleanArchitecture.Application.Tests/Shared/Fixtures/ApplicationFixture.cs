using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dapper.CleanArchitecture.Application.Tests.Shared.Fixtures;

public class ApplicationFixture : DbFixture
{
    public EventHandler<IServiceCollection> OnConfigureServices { get; set; }

    private ServiceProvider _provider;

    private ServiceProvider Provider
    {
        get
        {
            if (_provider != null)
            {
                return _provider;
            }

            _services.AddTransient(typeof(ILogger<>), typeof(NullLogger<>));
            Context.Setup(x => x.Connection).Returns(DbConnection.Object);
            ReadContext.Setup(x => x.Connection).Returns(DbConnection.Object);
            _services.AddTransient(_ => Context.Object);
            _services.AddTransient(_ => ReadContext.Object);
            _services.AddApplication();
            OnConfigureServices?.Invoke(this, _services);
            _provider = _services.BuildServiceProvider();

            return _provider;
        }
    }

    private readonly ServiceCollection _services = new();

    public async Task<T> SendAsync<T>(IRequest<T> request)
    {
        var mediator = Provider.GetService<IMediator>();

        if (mediator == null)
        {
            throw new MediatorFixtureConfigurationException();
        }

        return await mediator.Send(request);
    }
}

public class MediatorFixtureConfigurationException : Exception
{
    public MediatorFixtureConfigurationException() : base("MediatR was not properly initialized in the service collection")
    {
    }
}