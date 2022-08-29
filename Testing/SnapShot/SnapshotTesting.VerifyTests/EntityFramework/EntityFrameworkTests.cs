using Microsoft.EntityFrameworkCore;
using SnapshotTesting.VerifyTests.EntityFramework.Context;
using SnapshotTesting.VerifyTests.EntityFramework.Fixtures;
using VerifyTests.EntityFramework;

namespace SnapshotTesting.VerifyTests.EntityFramework;

[UsesVerify]
public class EntityFrameworkTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public EntityFrameworkTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetEntitiesAsyncTest()
    {
        await using var dbContext = await DbContextBuilder.GetDbContext(_fixture.Container.ConnectionString);
        await DbContextBuilder.SeedDatabase(dbContext);

        EfRecording.StartRecording();

        var companies = await dbContext.Companies.Where(x => x.Content == "Company2").ToListAsync();
        await Verify(companies);

        // Wipe Database
        await dbContext.Reset();
    }

    [Fact]
    public async Task AddEntityToDbTest()
    {
        await using var dbContext = await DbContextBuilder.GetDbContext(_fixture.Container.ConnectionString);
        EfRecording.StartRecording();

        var employee1 = new Employee
        {
            Content = "Mr. John"
        };
        var company = new Company
        {
            Content = "Company A",
            Employees = new List<Employee> { employee1 }
        };
        dbContext.Add(company);
        await dbContext.SaveChangesAsync();

        var companies = await dbContext.Companies.Where(x => x.Content == "Company A").ToListAsync();
        await Verify(companies);

        // Wipe Database
        await dbContext.Reset();
    }
}