using Quartz;

namespace QuartzExample.Web.Extensions;

public static class QuartzScheduleJobExtensions
{
    public static void ScheduleImmediateJob<T>(this IScheduler scheduler) where T : IJob
    {
        scheduler.ScheduleImmediateJob<T>(new Dictionary<string, string>());
    }

    public static void ScheduleImmediateJob<T>(this IScheduler scheduler, Dictionary<string, string> values) where T : IJob
    {
        var jobBuilder = JobBuilder.Create<T>();

        foreach (var value in values)
        {
            jobBuilder.UsingJobData(value.Key, value.Value);
        }

        scheduler.ScheduleJob(jobBuilder.Build(), TriggerBuilder.Create().StartNow().Build());
    }

    public static void ScheduleDelayedJob<T>(this IScheduler scheduler, TimeSpan span) where T : IJob
    {
        scheduler.ScheduleDelayedJob<T>(span, new Dictionary<string, string>());
    }

    public static void ScheduleDelayedJob<T>(this IScheduler scheduler, TimeSpan span, Dictionary<string, string> values) where T : IJob
    {
        var jobBuilder = JobBuilder.Create<T>();

        foreach (var value in values)
        {
            jobBuilder.UsingJobData(value.Key, value.Value);
        }

        scheduler.ScheduleJob(jobBuilder.Build(), TriggerBuilder.Create().StartAt(DateTimeOffset.Now.Add(span)).Build());
    }
}