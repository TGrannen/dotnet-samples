using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzExample.Web.Jobs;
using System;

namespace QuartzExample.Web
{
    public static class QuartzHelpers
    {
        public static void ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QuartzOptions>(configuration.GetSection("BackgroundTasks:Quartz"));

            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";

                q.UseMicrosoftDependencyInjectionScopedJobFactory(options =>
                {
                    // if we don't have the job in DI, allow fallback
                    // to configure via default constructor
                    options.AllowDefaultConstructor = true;
                });

                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });
            });

            AddAllJobsToServiceCollection(services);

            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            services.AddTransient(provider => provider.GetRequiredService<ISchedulerFactory>().GetScheduler().GetAwaiter().GetResult());
        }

        public static void ScheduleQuartzJobs(this IServiceProvider provider, IConfiguration configuration, ILogger logger)
        {
            var factory = provider.GetRequiredService<ISchedulerFactory>();
            var scheduler = factory.GetScheduler().GetAwaiter().GetResult();

            scheduler.ConfigureJobWithCronSchedule<ExampleJob1>(logger, configuration, "BackgroundTasks:ExampleJob1CronExpression");
        }

        private static void AddAllJobsToServiceCollection(IServiceCollection services)
        {
            services.Scan(scan => scan.FromCallingAssembly().AddClasses(x => x.AssignableTo<IJob>()));
        }

        private static void ConfigureJobWithCronSchedule<T>(this IScheduler scheduler, ILogger logger, IConfiguration configuration, string configurationSetting) where T : IJob
        {
            var cronExpression = configuration.GetValue(configurationSetting, "");
            scheduler.ConfigureJobWithCronSchedule<T>(logger, cronExpression);
        }

        private static void ConfigureJobWithCronSchedule<T>(this IScheduler scheduler, ILogger logger, string cronExpression) where T : IJob
        {
            if (!string.IsNullOrEmpty(cronExpression))
            {
                logger.LogInformation("Configuring {Job} Job with schedule: {CronSchedule}", typeof(T).Name, cronExpression);
                IJobDetail job = JobBuilder.Create<T>().WithIdentity(typeof(T).FullName).Build();
                ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExpression).Build();
                scheduler.ScheduleJob(job, trigger).GetAwaiter().GetResult();
            }
            else
            {
                logger.LogWarning("Not running {Job} due to missing Cron Expression", typeof(T).Name);
            }
        }
    }
}