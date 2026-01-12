using Microsoft.EntityFrameworkCore;
using Npgsql;
using SnapshotTesting.VerifyTests.EntityFramework.Fixtures;

namespace SnapshotTesting.VerifyTests.EntityFramework.Context;

public static class SampleDbContextExtensions
{
    public static async Task Reset(this SampleDbContext dbContext, PostgresFixture fixture)
    {
        var conn = dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        var res = await fixture.GetRespawner();
        await res.ResetAsync(conn);
    }
}