using Microsoft.EntityFrameworkCore;
using SnapshotTesting.VerifyTests.EntityFramework.Context;
using SnapshotTesting.VerifyTests.EntityFramework.Fixtures;
using VerifyTests.EntityFramework;

namespace SnapshotTesting.VerifyTests.EntityFramework;

public class EntityFrameworkTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task GetEntitiesAsyncTest()
    {
        await using var dbContext = await DbContextBuilder.GetDbContext(fixture.Container.GetConnectionString());
        await DbContextBuilder.SeedDatabase(dbContext);

        Recording.Start();

        var companies = await dbContext.Companies.Where(x => x.Content == "Company2").ToListAsync();
        await Verify(companies);

        // Wipe Database
        await dbContext.Reset(fixture);
    }

    [Fact]
    public async Task AddEntityToDbTest()
    {
        await using var dbContext = await DbContextBuilder.GetDbContext(fixture.Container.GetConnectionString());
        Recording.Start();

        var employee1 = new Employee
        {
            Content = "Mr. John"
        };
        var company = new Company
        {
            Content = "Company A",
            Employees = [employee1]
        };
        dbContext.Add(company);
        await dbContext.SaveChangesAsync();

        var companies = await dbContext.Companies.Where(x => x.Content == "Company A").ToListAsync();
        await Verify(companies);

        // Wipe Database
        await dbContext.Reset(fixture);
    }
}