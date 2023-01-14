namespace FeatureFlags.Flagsmith.WebAPI.Features;

class HardCodedTestContextProvider : IContextProvider
{
    public Task<IFeatureContext> GetUserAsync()
    {
        return Task.FromResult(
            new FeatureContext
            {
                Key = "TEST",
                Email = new ContextAttribute<string>("test@test.com"),
                CustomContextAttributes = new List<CustomContextAttribute<string>>
                {
                    new("group", "admins")
                }
            } as IFeatureContext);
    }
}