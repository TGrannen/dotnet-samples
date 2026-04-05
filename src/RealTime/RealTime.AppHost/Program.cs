var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.RealTime_Api>("realtime-api")
    .WithReference(redis)
    .WaitFor(redis);

builder.AddProject<Projects.RealTime_Web>("realtime-web")
    .WithExternalHttpEndpoints()
    .WithReference(api);

builder.Build().Run();
