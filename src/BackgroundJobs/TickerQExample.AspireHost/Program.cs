using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("Database")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

builder.AddProject<TickerQExample_WebAPI>("API")
    .WithEnvironment("InstanceIdentifier", "API1")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<TickerQExample_WebAPI>("API2")
    .WithEnvironment("InstanceIdentifier", "API2")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();