var builder = DistributedApplication.CreateBuilder(args);

var efdb = builder.AddPostgres("ef-pgsql").AddDatabase("ef-test-db");
var db = builder.AddPostgres("dapper-pgsql").AddDatabase("test-db");

builder.AddProject<Projects.EFCore_Web>("efcore-web")
    .WithReference(efdb)
    .WaitFor(efdb);

builder.Build().Run();