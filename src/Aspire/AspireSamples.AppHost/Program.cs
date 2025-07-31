var builder = DistributedApplication.CreateBuilder(args);

var seq = builder
    .AddSeq("seq", 5341)
    .WithDataVolume()
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithLifetime(ContainerLifetime.Persistent)
    ;

var cache = builder
    .AddRedis("rediscache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(resourceBuilder => resourceBuilder.WithHostPort(51500).WithLifetime(ContainerLifetime.Persistent))
    .WithPersistence();

var apiService = builder.AddProject<Projects.AspireSamples_ApiService>("apiservice")   
    .WithReference(seq)
    .WaitFor(seq);

builder.AddProject<Projects.AspireSamples_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(cache)
    .WithReference(seq)
    .WaitFor(cache)
    .WaitFor(seq)
    .WaitFor(apiService);

builder.Build().Run();