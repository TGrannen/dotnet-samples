using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GettingStarted;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                
                services.Configure<TestingConfig>(hostContext.Configuration.GetSection("TestingConfig"));
                services.AddMassTransit(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    // By default, sagas are in-memory, but should be changed to a durable
                    // saga repository.
                    x.SetInMemorySagaRepositoryProvider();

                    var entryAssembly = Assembly.GetEntryAssembly();

                    x.AddConsumers(entryAssembly);
                    x.AddSagaStateMachines(entryAssembly);
                    x.AddSagas(entryAssembly);
                    x.AddActivities(entryAssembly);

                    // x.UsingInMemory((context, cfg) =>
                    // {
                    //     cfg.ConfigureEndpoints(context);
                    // });

                    // elided ...
                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        var scope = hostContext.Configuration.GetValue<string>("scope");
                        cfg.Host("us-east-2", h =>
                        {
                            h.AccessKey(hostContext.Configuration.GetValue<string>("your-iam-access-key"));
                            h.SecretKey(hostContext.Configuration.GetValue<string>("your-iam-secret-key"));

                            h.Scope(scope, true);
                        });

                        cfg.ConfigureEndpoints(context, new DefaultEndpointNameFormatter($"{scope}-", false));
                    });
                });

                services.AddHostedService<Worker>();
            })
            .UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
}