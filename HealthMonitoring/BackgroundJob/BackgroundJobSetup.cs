using Microsoft.Extensions.Options;
using Quartz;

namespace HealthMonitoring.BackgroundJob;

public class BackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
    private static readonly TimeSpan TimeInMinutes = TimeSpan.FromMinutes(1);
    
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(BackgroundJob));

        options
            .AddJob<BackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(TimeInMinutes.Minutes).RepeatForever()));
    }
}