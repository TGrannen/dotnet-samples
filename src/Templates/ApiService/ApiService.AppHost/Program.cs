var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var postgresDb = postgres.AddDatabase("apiservice");

var redis = builder.AddRedis("cache")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(x => x.WithLifetime(ContainerLifetime.Persistent).WithDataVolume());

var migrator = builder.AddProject<Projects.ApiService_Migrator>("apiservice-migrator")
    .WithReference(postgresDb, connectionName: "DefaultConnection")
    .WaitFor(postgres);

builder.AddProject<Projects.ApiService_Api>("apiservice-api")
    .WithReference(postgresDb, connectionName: "DefaultConnection")
    .WithReference(redis)
    .WaitFor(postgresDb)
    .WaitFor(redis)
    .WaitForCompletion(migrator);

builder.Build().Run();
