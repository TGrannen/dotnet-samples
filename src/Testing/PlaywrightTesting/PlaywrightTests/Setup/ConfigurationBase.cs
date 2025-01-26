namespace PlaywrightTests.Setup;

public abstract class ConfigurationBase : ExpectBase
{
    protected IConfigurationRoot? Configuration { get; private set; }

    [OneTimeSetUp]
    public void BaseSetUp()
    {
        DotNetEnv.Env.Load();
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<PageBase>()
            .AddEnvironmentVariables()
            .Build();
    }
}