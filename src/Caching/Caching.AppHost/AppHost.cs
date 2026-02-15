var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("caching-redis-data")
    .WithRedisInsight(resourceBuilder => resourceBuilder.WithLifetime(ContainerLifetime.Persistent));

builder.AddProject<Projects.Caching_WebAPI>("caching-webapi")
    .WithReference(cache);

builder.Build().Run();
