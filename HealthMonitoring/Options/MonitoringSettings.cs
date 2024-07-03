namespace HealthMonitoring.Options;

public class MonitoringSettings
{
    public const string SectionName = "MonitoringSettings";
    
    public string MachineName { get; set; } = string.Empty;
    
    public int RamMemoryThresholdInPercent { get; set; }
    
    public int RomMemoryThresholdInPercent { get; set; }
}