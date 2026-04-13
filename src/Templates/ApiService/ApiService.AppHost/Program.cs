var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var postgresDb = postgres.AddDatabase("apiservice");

var migrator = builder.AddProject<Projects.ApiService_Migrator>("apiservice-migrator")
    .WithReference(postgresDb, connectionName: "DefaultConnection")
    .WaitFor(postgres);

builder.AddProject<Projects.ApiService_Api>("apiservice-api")
    .WithReference(postgresDb, connectionName: "DefaultConnection")
    .WaitFor(postgresDb)
    .WaitForCompletion(migrator);

builder.Build().Run();
