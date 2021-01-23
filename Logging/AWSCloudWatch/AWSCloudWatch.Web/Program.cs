using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace AWSCloudWatch.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                    configuration.WriteTo.WriteToAWS(context.Configuration, context.HostingEnvironment);

                    // To see internal Serilog errors that contain messages about not properly
                    // communicating to AWS
                    Serilog.Debugging.SelfLog.Enable(Console.Error);
                    //Serilog.Debugging.SelfLog.Enable(s => Debug.WriteLine(s));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}