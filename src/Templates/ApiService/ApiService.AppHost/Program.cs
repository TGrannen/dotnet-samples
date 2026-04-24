var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var sqlDb = sql.AddDatabase("apiservice");

var redis = builder.AddRedis("cache")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(x => x.WithLifetime(ContainerLifetime.Persistent).WithDataVolume());

var migrator = builder.AddProject<Projects.ApiService_Migrator>("apiservice-migrator")
    .WithReference(sqlDb, connectionName: "DefaultConnection")
    .WaitFor(sql);

builder.AddProject<Projects.ApiService_Api>("apiservice-api")
    .WithReference(sqlDb, connectionName: "DefaultConnection")
    .WithReference(redis)
    .WaitFor(sqlDb)
    .WaitFor(redis)
    .WaitForCompletion(migrator);

builder.Build().Run();
