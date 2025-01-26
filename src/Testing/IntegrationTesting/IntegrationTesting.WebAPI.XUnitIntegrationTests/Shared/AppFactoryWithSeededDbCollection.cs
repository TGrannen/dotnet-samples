namespace IntegrationTesting.WebAPI.XUnitIntegrationTests.Shared;

[CollectionDefinition(nameof(AppFactoryWithSeededDbCollection))]
public class AppFactoryWithSeededDbCollection : ICollectionFixture<AppFactoryWithSeededDb<Program>>
{
}