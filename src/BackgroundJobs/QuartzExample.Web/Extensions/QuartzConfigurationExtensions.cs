using Quartz;

namespace QuartzExample.Web.Extensions;

public static class QuartzConfigurationExtensions
{
    public static void ConfigureJobWithCronSchedule<T>(this IScheduler scheduler, ILogger logger, string cronExpression) where T : IJob
    {
        if (!string.IsNullOrEmpty(cronExpression))
        {
            logger.LogInformation("Configuring {Job} Job with schedule: {CronSchedule}", typeof(T).Name, cronExpression);
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(typeof(T).FullName!).Build();
            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExpression).Build();
            scheduler.ScheduleJob(job, trigger).GetAwaiter().GetResult();
        }
        else
        {
            logger.LogWarning("Not running {Job} due to missing Cron Expression", typeof(T).Name);
        }
    }

    public static void ConfigureJobWithCronSchedule<T>(this IScheduler scheduler, ILogger logger, IConfiguration configuration, string configurationSetting) where T : IJob
    {
        var cronExpression = configuration.GetValue(configurationSetting, "");
        scheduler.ConfigureJobWithCronSchedule<T>(logger, cronExpression);
    }

    public static void ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QuartzOptions>(configuration.GetSection("BackgroundTasks:Quartz"));

        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";

            q.UseMicrosoftDependencyInjectionJobFactory(options =>
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

        // Add all jobs to IOC Container
        services.Scan(scan => scan.FromCallingAssembly().AddClasses(x => x.AssignableTo<IJob>()));

        services.AddQuartzHostedService(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        //TODO: This could probably be improved but works for now
        services.AddTransient(provider => provider.GetRequiredService<ISchedulerFactory>().GetScheduler().GetAwaiter().GetResult());
    }
}