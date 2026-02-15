var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Caching_WebAPI>("caching-webapi");

builder.Build().Run();
