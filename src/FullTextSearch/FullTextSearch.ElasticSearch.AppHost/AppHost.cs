var builder = DistributedApplication.CreateBuilder(args);

var elastic = builder
    .AddElasticsearch("elasticsearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("xpack.security.enabled", "false")
    .WithEnvironment("discovery.type", "single-node");

builder
    .AddContainer("kibana", "docker.elastic.co/kibana/kibana", "8.14.1")
    .WithEnvironment("ELASTICSEARCH_HOSTS", elastic.GetEndpoint("http"))
    .WithHttpEndpoint(targetPort: 5601, name: "http")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithReference(elastic)
    .WaitFor(elastic);

builder.AddProject<Projects.FullTextSearch_ElasticSearch_Web>("api")
    .WithReference(elastic)
    .WaitFor(elastic);

builder.Build().Run();