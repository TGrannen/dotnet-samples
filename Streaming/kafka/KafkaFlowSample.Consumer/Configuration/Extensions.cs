using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using KafkaFlowSample.Consumer.Middleware;

namespace KafkaFlowSample.Consumer.Configuration;

public static class Extensions
{
    public static IHostApplicationBuilder SetupKafka(this IHostApplicationBuilder builder,
        Dictionary<string, Action<IConsumerMiddlewareConfigurationBuilder>>? middlewareActionDict = null)
    {
        middlewareActionDict ??= new Dictionary<string, Action<IConsumerMiddlewareConfigurationBuilder>>();
        var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:messaging");
        var configDict = builder.Configuration.GetSection("ConsumerConfig").Get<Dictionary<string, ConsumerConfigurationTest>>();

        builder.Services.AddKafkaFlowHostedService(kafka => kafka
            .UseMicrosoftLog()
            .AddOpenTelemetryInstrumentation()
            .AddCluster(cluster =>
            {
                cluster
                    .WithBrokers(new[] { connectionString })
                    .OnStarted(resolver => { resolver.Resolve<ILogger<Program>>().LogInformation("Started Kafka Cluster"); })
                    .OnStopping(resolver => { resolver.Resolve<ILogger<Program>>().LogInformation("Stopping Kafka Cluster"); });

                if (builder.Environment.IsDevelopment())
                {
                    cluster
                        .EnableAdminMessages("kafka-flow.admin").CreateTopicIfNotExists("kafka-flow.admin", 1, 1)
                        .EnableTelemetry("kafka-flow.admin.telemetry").CreateTopicIfNotExists("kafka-flow.admin.telemetry", 1, 1);
                }

                foreach (var (name, config) in configDict!)
                {
                    cluster.SetupConsumerViaConfig(name, config, builder.Environment.IsDevelopment(), middlewareActionDict);
                }
            })
        );
        return builder;
    }

    public static IClusterConfigurationBuilder SetupConsumerViaConfig(this IClusterConfigurationBuilder cluster,
        string name,
        ConsumerConfigurationTest config,
        bool createTopics,
        Dictionary<string, Action<IConsumerMiddlewareConfigurationBuilder>> middlewareActionDict)
    {
        if (createTopics)
        {
            foreach (var topic in config.Topics)
            {
                cluster.CreateTopicIfNotExists(topic, 3, 1);
            }
        }

        cluster.AddConsumer(consumer =>
        {
            consumer
                .WithName(name)
                .Topics(config.Topics)
                .WithGroupId(config.ConsumerGroupId)
                .WithBufferSize(config.BufferSize)
                .WithWorkersCount(config.WorkerCount)
                .WithWorkerStopTimeout(config.WorkerStoppedTimeout);

            var isBatching = config.Batching != null;
            if (isBatching)
            {
                consumer.WithManualMessageCompletion();
            }

            if (isBatching)
            {
                middlewareActionDict.TryGetValue(name, out var action);
                consumer.WithManualMessageCompletion();
                consumer.AddMiddlewares(middlewares =>
                {
                    middlewares
                        .SetupDeserializer(config)
                        .AddBatching(config.Batching!.BatchSize, config.Batching.Timeout)
                        .Add<ErrorMiddleware>(MiddlewareLifetime.Message)
                        .Add<BatchCompletionProcessingMiddleware>();

                    action?.Invoke(middlewares);
                });
            }
            else
            {
                consumer.AddMiddlewares(middlewares =>
                {
                    middlewares
                        .SetupDeserializer(config)
                        .Add<ErrorMiddleware>(MiddlewareLifetime.Message)
                        .AddTypedHandlers(h => h
                            .AddHandlersFromAssemblyOf<Program>()
                            .WithHandlerLifetime(InstanceLifetime.Scoped));
                });
            }
        });
        return cluster;
    }

    private static IConsumerMiddlewareConfigurationBuilder SetupDeserializer(this IConsumerMiddlewareConfigurationBuilder middlewares,
        ConsumerConfigurationTest config)
    {
        if (!string.IsNullOrWhiteSpace(config.CdcTypeName))
        {
            var cdcResolver = new CdcMessageTypeResolver(config.CdcTypeName);
            middlewares.AddDeserializer(resolver => resolver.Resolve<JsonCoreDeserializer>(), _ => cdcResolver);
        }
        else if (!string.IsNullOrWhiteSpace(config.StaticTypeName))
        {
            var staticMessageTypeResolver = new StaticMessageTypeResolver(config.StaticTypeName);
            middlewares.AddDeserializer(resolver => resolver.Resolve<JsonCoreDeserializer>(), _ => staticMessageTypeResolver);
        }
        else
        {
            middlewares.AddDeserializer<JsonCoreDeserializer>();
        }

        return middlewares;
    }
}