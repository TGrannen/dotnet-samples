using Microsoft.Extensions.DependencyInjection;

namespace Mocking.Mediator.Tests.Fixtures
{
    public class ApplicationFixture : MediatorFixture
    {
        public override void AddBindings(IServiceCollection services)
        {
            services.AddApplication();
        }
    }
}