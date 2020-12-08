using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mocking.Mediator.Tests.Exceptions;
using Mocking.Moq.Extensions;
using Mocking.Moq.Loggers;
using System.Threading.Tasks;

namespace Mocking.Mediator.Tests.Fixtures
{
    public class MediatorFixture
    {
        public ServiceProvider Provider { get; private set; }

        protected ServiceCollection Services { get; } = new ServiceCollection();

        public virtual void AddBindings(IServiceCollection services)
        {
        }

        public async Task<T> SendAsync<T>(IRequest<T> request)
        {
            if (Provider == null)
            {
                Services.AddMockedLogging();
                AddBindings(Services);
                Provider = Services.BuildServiceProvider();
            }

            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            return await mediator.Send(request);
        }

        public MockedILogger<T> GetMockedLogger<T>()
        {
            return Provider.GetMockedLogger<T>();
        }
    }
}