using Messaging.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Configuration;

public static class DependencyInjection
{
    public static void AddAwsSqsConfiguration(this IServiceCollection services, IConfiguration configuration, string section = "AwsSqsConfig")
    {
        services.Configure<AwsSqsConfig>(configuration.GetSection(section));

        services.AddTransient<IAwsSqsConfig>(provider => provider.GetRequiredService<IOptions<AwsSqsConfig>>().Value);
    }
}