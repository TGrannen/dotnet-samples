var builder = DistributedApplication.CreateBuilder(args);

var efdb = builder.AddPostgres("ef-pgsql").WithLifetime(ContainerLifetime.Persistent).AddDatabase("ef-test-db");
var db = builder.AddPostgres("dapper-pgsql").WithLifetime(ContainerLifetime.Persistent).AddDatabase("test-db");

builder.AddProject<Projects.EFCore_Web>("efcore-web")
    .WithReference(efdb)
    .WaitFor(efdb);

builder.Build().Run();