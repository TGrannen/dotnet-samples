namespace Mocking.Mediator.Tests.Fixtures
{
    public class ApplicationFixture : MediatorFixture
    {
        public ApplicationFixture()
        {
            OnConfigureServices += (_, services) =>
            {
                services.AddApplication();
            };
        }
    }
}