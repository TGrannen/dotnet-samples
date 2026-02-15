var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("caching-redis-data")
    .WithRedisInsight(resourceBuilder => resourceBuilder
        .WithLifetime(ContainerLifetime.Persistent)
        .WithHostPort(59268));

builder.AddProject<Projects.Caching_WebAPI>("caching-webapi")
    .WithReference(cache);

builder.Build().Run();