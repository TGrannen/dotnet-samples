using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Outbox.SampleBlazor.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddDynamoDb(this IServiceCollection services, IConfiguration config)
    {
        var serviceUrl = config.GetValue("AWS:Dynamo:LocalUrl", string.Empty);
        if (string.IsNullOrEmpty(serviceUrl))
        {
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            return services;
        }

        services.AddTransient<ISeeder, Seeder>();
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>(opt =>
            new DynamoDBContext(new AmazonDynamoDBClient("xxx", "xxx",
                new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1,
                    UseHttp = true,
                    ServiceURL = serviceUrl
                })));

        services.AddAWSService<IAmazonDynamoDB>(options: new AWSOptions
        {
            Region = Amazon.RegionEndpoint.USEast1,
            Credentials = new BasicAWSCredentials("xxx", "xxx")
        });

        return services;
    }

    public static async Task<IContainer> BuildLocalStackContainer(this WebApplicationBuilder builder)
    {
        var container = new ContainerBuilder()
            .WithImage("localstack/localstack")
            .WithName("outbox-localstack-demo")
            .WithCleanUp(true)
            .WithEnvironment("DEFAULT_REGION", "us-east-1")
            .WithEnvironment("SERVICES", "dynamodb")
            .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
            .WithEnvironment("DEBUG", "1")
            .WithEnvironment("DYNAMODB_SHARE_DB", "1")
            .WithEnvironment("DATA_DIR", "/tmp/localstack/data")
            .WithPortBinding(4566, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(4566)).Build();

        await container.StartAsync();

        builder.Configuration.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>("AWS:Dynamo:LocalUrl", $"http://localhost:{container.GetMappedPublicPort(4566)}")
        });

        return container;
    }
}