namespace TestProject1IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

[CollectionDefinition(nameof(AppFactoryWithDbCollection))]
public class AppFactoryWithDbCollection : ICollectionFixture<AppFactoryWithDb<Program>>
{
}