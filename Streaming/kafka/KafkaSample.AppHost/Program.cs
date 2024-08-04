var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddKafka("messaging")
    .WithDataVolume()
    .WithKafkaUI();

var apiService = builder.AddProject<Projects.KafkaSample_ApiService>("apiservice")
    .WithReference(messaging);

var consumer = builder.AddProject<Projects.KafkaSample_Consumer>("consumer")
    .WithReference(messaging);

builder.AddProject<Projects.KafkaSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(messaging)
    .WithReference(apiService);

builder.Build().Run();