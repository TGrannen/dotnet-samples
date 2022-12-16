using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.Split.WebAPI.Features;

class HardCodedTestContextProvider : IContextProvider
{
    public Task<IFeatureContext> GetUserAsync()
    {
        return Task.FromResult(
            new FeatureContext
            {
                Key = "TEST",
                Email = new ContextAttribute<string>("test@test.com")
            } as IFeatureContext);
    }
}