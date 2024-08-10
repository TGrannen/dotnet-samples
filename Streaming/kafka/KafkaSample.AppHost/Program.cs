var builder = DistributedApplication.CreateBuilder(args);

// var messaging = builder.AddKafka("messaging")
//     .WithDataVolume()
//     .WithKafkaUI();

// Add the kafka broker
var messaging = builder.AddContainer("broker", "confluent-local", "7.6.0")
                    .WithImageRegistry("confluentinc")
                    .WithEndpoint(name: "primary", targetPort: 9092)
                    .WithEndpoint(targetPort: 9101);

// Add the schema registry
var schemaRegistry = builder.AddContainer("schema-registry", "cp-schema-registry", "7.6.1")
                            .WithImageRegistry("confluentinc")
                            .WithHttpEndpoint(name: "primary", targetPort: 8081);

messaging.WithEnvironment(context =>
{
    var selfEndpoint = messaging.GetEndpoint("primary");

    // Set the advertised listeners to the public port
    var advertisedListeners = builder.ExecutionContext.IsRunMode
        // This is a workaround for https://github.com/dotnet/aspire/issues/3735
        ? ReferenceExpression.Create($"PLAINTEXT://localhost:29092,PLAINTEXT_HOST://{selfEndpoint.ContainerHost}:{selfEndpoint.Property(EndpointProperty.Port)}")
        : ReferenceExpression.Create($"PLAINTEXT://{selfEndpoint.Property(EndpointProperty.Host)}:29092,PLAINTEXT_HOST://{selfEndpoint.Property(EndpointProperty.Host)}:{selfEndpoint.Property(EndpointProperty.Port)}");

    context.EnvironmentVariables["KAFKA_ADVERTISED_LISTENERS"] = advertisedListeners;

    // Set the URL to the schema registry
    context.EnvironmentVariables["KAFKA_SCHEMA_REGISTRY_URL"] = schemaRegistry.GetEndpoint("primary");
});

schemaRegistry.WithEnvironment(context =>
{
    // Get the broker endpoint
    var brokerEndpoint = messaging.GetEndpoint("primary");

    // Create a reference to the broker endpoint, doing it this way will ensure that it works in both run and publish mode
    var brokerConnectionReference = builder.ExecutionContext.IsRunMode
        // This is a workaround for https://github.com/dotnet/aspire/issues/3735
        ? ReferenceExpression.Create($"{brokerEndpoint.ContainerHost}:{brokerEndpoint.Property(EndpointProperty.Port)}")
        : ReferenceExpression.Create($"{brokerEndpoint.Property(EndpointProperty.Host)}:{brokerEndpoint.Property(EndpointProperty.Port)}");

    context.EnvironmentVariables["SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS"] = brokerConnectionReference;
    context.EnvironmentVariables["SCHEMA_REGISTRY_HOST_NAME"] = "schema-registry";
});

var apiService = builder.AddProject<Projects.KafkaSample_ApiService>("apiservice")
    .WithEnvironment("ConnectionStrings__messaging", messaging.GetEndpoint("primary"))
    .WithEnvironment("ConnectionStrings__schema", schemaRegistry.GetEndpoint("primary"));

var consumer = builder.AddProject<Projects.KafkaSample_Consumer>("consumer")
    .WithEnvironment("ConnectionStrings__messaging", messaging.GetEndpoint("primary"));

builder.AddProject<Projects.KafkaSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ConnectionStrings__messaging", messaging.GetEndpoint("primary"))
    .WithReference(apiService);

builder.Build().Run();