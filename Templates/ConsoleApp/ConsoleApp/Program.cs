using ConsoleApp.Commands;
using ConsoleApp.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var config = InitConfig();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton<ICommand, HelloWorldCommand>()
                .BuildServiceProvider();

            var commandLineApp = new CommandLineApplicationWithDI(serviceProvider);
            return commandLineApp.Execute(args);
        }

        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}