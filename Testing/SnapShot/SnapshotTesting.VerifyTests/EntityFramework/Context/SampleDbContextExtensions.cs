using Microsoft.EntityFrameworkCore;
using SnapshotTesting.VerifyTests.EntityFramework.Fixtures;

namespace SnapshotTesting.VerifyTests.EntityFramework.Context;

public static class SampleDbContextExtensions
{
    public static async Task Reset(this SampleDbContext dbContext)
    {
        var conn = dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        await PostgresFixture.Checkpoint.Reset(conn);
    }
}