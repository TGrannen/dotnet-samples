var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("caching-redis-data")
    .WithRedisInsight(resourceBuilder => resourceBuilder
        .WithLifetime(ContainerLifetime.Persistent)
        .WithHostPort(59268));

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("caching-postgres-data");
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Caching_WebAPI>("caching-webapi")
    .WithReference(cache)
    .WaitFor(postgresdb)
    .WithReference(postgresdb);

builder.Build().Run();