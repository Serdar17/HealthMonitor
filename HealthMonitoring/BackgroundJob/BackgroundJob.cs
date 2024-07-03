using HealthMonitoring.Services.Interfaces;
using Quartz;

namespace HealthMonitoring.BackgroundJob;

public class BackgroundJob : IJob
{
    private readonly ILogger<BackgroundJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public BackgroundJob(ILogger<BackgroundJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("background working started at {StartTime}", DateTime.Now);
        var scope = _serviceProvider.CreateScope();
        var ramMonitor = scope.ServiceProvider.GetService<IRamMonitor>();
        var romMonitor = scope.ServiceProvider.GetService<IRomMonitor>();
        
        await ramMonitor!.Check();
        await romMonitor!.Check();
        
        _logger.LogInformation("background working ended at {EndTime}", DateTime.Now);
    }
}