using HealthMonitoring.BackgroundJob;
using Quartz;

namespace HealthMonitoring.Configuration;

public static class BackgroundJobConfiguration
{
    public static IServiceCollection AddBackgroundJob(this IServiceCollection services)
    {
        services.AddQuartz(opt => opt.UseMicrosoftDependencyInjectionJobFactory());

        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

        services.ConfigureOptions<BackgroundJobSetup>();

        return services;
    }
}