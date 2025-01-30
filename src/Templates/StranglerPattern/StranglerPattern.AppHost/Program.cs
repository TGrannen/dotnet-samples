var builder = DistributedApplication.CreateBuilder(args);

var oldApi = builder.AddProject<Projects.StranglerPattern_OldApiService>("oldapi");
builder.AddProject<Projects.StranglerPattern_ApiService>("apiservice")
    .WithReference(oldApi)
    .WaitFor(oldApi);

builder.Build().Run();