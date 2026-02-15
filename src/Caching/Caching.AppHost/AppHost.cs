var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight();

builder.AddProject<Projects.Caching_WebAPI>("caching-webapi")
    .WithReference(cache);

builder.Build().Run();
