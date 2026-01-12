using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using VerifyTests.EntityFramework;

namespace SnapshotTesting.VerifyTests.EntityFramework.Context;

public static class DbContextBuilder
{
    public static async Task<SampleDbContext> GetDbContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<SampleDbContext>()
            .EnableRecording()
            .UseNpgsql(connectionString);
        var context = new SampleDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    public static async Task SeedDatabase(SampleDbContext data)
    {
        if (data.Companies.Any()) return;

        var company1 = new Company
        {
            Id = 1,
            Content = "Company1"
        };
        var employee1 = new Employee
        {
            Id = 2,
            CompanyId = company1.Id,
            Content = "Employee1",
            Age = 25
        };
        var employee2 = new Employee
        {
            Id = 3,
            CompanyId = company1.Id,
            Content = "Employee2",
            Age = 31
        };
        var company2 = new Company
        {
            Id = 4,
            Content = "Company2"
        };
        var employee4 = new Employee
        {
            Id = 5,
            CompanyId = company2.Id,
            Content = "Employee4",
            Age = 34
        };
        var company3 = new Company
        {
            Id = 6,
            Content = "Company3"
        };
        var company4 = new Company
        {
            Id = 7,
            Content = "Company4"
        };
        data.AddRange(company1, employee1, employee2, company2, company3, company4, employee4);
        await data.SaveChangesAsync();
    }
}