using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SnapshotTesting.VerifyTests.EntityFramework.Context;

namespace SnapshotTesting.VerifyTests;

public static class Initialization
{
    [ModuleInitializer]
    public static void EnableVerifyExtensions()
    {
        VerifyMicrosoftLogging.Initialize();
        VerifyHttp.Initialize();
        VerifyMoq.Initialize();
        VerifyNewtonsoftJson.Initialize();
        VerifyEntityFramework.Initialize(GetDbModel());
    }

    private static IModel GetDbModel()
    {
        var options = new DbContextOptionsBuilder<SampleDbContext>();
        options.UseNpgsql("fake");
        using var data = new SampleDbContext(options.Options);
        return data.Model;
    }
}