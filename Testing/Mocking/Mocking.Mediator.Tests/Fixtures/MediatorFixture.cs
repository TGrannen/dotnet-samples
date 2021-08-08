using System;
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
        private ServiceProvider _provider;
        protected ServiceCollection Services { get; } = new ServiceCollection();
        public EventHandler<IServiceCollection> OnConfigureServices { get; set; }

        public ServiceProvider Provider
        {
            get
            {
                if (_provider != null)
                {
                    return _provider;
                }

                Services.AddMockedLogging();
                OnConfigureServices?.Invoke(this, Services);
                Provider = Services.BuildServiceProvider();

                return _provider;
            }
            private set => _provider = value;
        }

        public async Task<T> SendAsync<T>(IRequest<T> request)
        {
            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            return await mediator.Send(request);
        }


        public async Task Publish(INotification notification)
        {
            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            await mediator.Publish(notification);
        }


        public MockedILogger<T> GetMockedLogger<T>()
        {
            return Provider.GetMockedLogger<T>();
        }
    }
}