namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests.Seeded;

[SetUpFixture]
public class SeededDatabaseSetupFixture
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("integrationTestDb")
        .WithUsername("postgres")
        .WithPassword("test-password")
        .Build();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await _container.StartAsync();
        await DbSeeder.CreateSchema(_container.GetConnectionString());
        await DbSeeder.SeedData(_container.GetConnectionString());
        SeededDatabaseSetupFixtureTestData.ConnectionString = _container.GetConnectionString();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container?.StopAsync()!;
    }
}