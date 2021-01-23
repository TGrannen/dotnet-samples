using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using System;

namespace AWSCloudWatch.Web
{
    public static class LoggingHelpers
    {
        public static void WriteToAWS(this LoggerSinkConfiguration writeTo, IConfiguration config, IHostEnvironment hostingEnvironment)
        {
            var settings = config.GetSection("Serilog:AWS").Get<AWSConfigSettings>();
            if (settings.LogToCloudWatch)
            {
                var options = SetupAWSLoggerOptions(hostingEnvironment.EnvironmentName, settings);
                var client = SetupAWSLoggerClient(settings);

                // Using nested sub-logger here to allow specific filtering rules to only be applied to this sink
                writeTo.Logger(lc =>
                {
                    lc.ReadFrom.Configuration(config, "Serilog:AWS");
                    lc.WriteTo.AmazonCloudWatch(options, client);
                });
            }
        }

        private static IAmazonCloudWatchLogs SetupAWSLoggerClient(AWSConfigSettings section)
        {
            var region = string.IsNullOrEmpty(section.Region)
                ? RegionEndpoint.USEast1
                : RegionEndpoint.GetBySystemName(section.Region);

            AWSCredentials credentials = new BasicAWSCredentials(section.AccessKey, section.SecretKey);
            IAmazonCloudWatchLogs client = new AmazonCloudWatchLogsClient(credentials, region);

            return client;
        }

        private static CloudWatchSinkOptions SetupAWSLoggerOptions(string environmentName, AWSConfigSettings section)
        {
            string awsLogGroupName = section.LogGroup;

            var options = new CloudWatchSinkOptions
            {
                LogGroupName = $"{awsLogGroupName}_{environmentName}",
                TextFormatter = new JsonFormatter(renderMessage: true),
                LogStreamNameProvider = new AppEnvDateLogStreamNameProvider(awsLogGroupName, environmentName),
                LogGroupRetentionPolicy = section.RetentionPolicy,
            };

            return options;
        }

        internal class AppEnvDateLogStreamNameProvider : ILogStreamNameProvider
        {
            private readonly string _appName;
            private readonly string _env;

            public AppEnvDateLogStreamNameProvider(string appName, string env)
            {
                _appName = appName;
                _env = env;
            }

            public string GetLogStreamName()
            {
                return $"{_appName}_{_env}_{DateTime.UtcNow:yyyy-MM}";
            }
        }

        private class AWSConfigSettings
        {
            public string AccessKey { get; set; }
            public string LogGroup { get; set; }
            public bool LogToCloudWatch { get; set; }
            public string Region { get; set; }
            public LogGroupRetentionPolicy RetentionPolicy { get; set; }
            public string SecretKey { get; set; }
        }
    }
}