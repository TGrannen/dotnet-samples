﻿using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace IntegrationTesting.WebAPI.NUnitIntegrationTests.DatabaseTests.Seeded;

[SetUpFixture]
public class SeededDatabaseSetupFixture
{
    private readonly PostgreSqlTestcontainer _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "integrationTestDb",
            Username = "postgres",
            Password = "test-password",
        }).Build();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await _container.StartAsync();
        await DbSeeder.CreateSchema(_container.ConnectionString);
        await DbSeeder.SeedData(_container.ConnectionString);
        SeededDatabaseSetupFixtureTestData.ConnectionString = _container.ConnectionString;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container?.StopAsync()!;
    }
}