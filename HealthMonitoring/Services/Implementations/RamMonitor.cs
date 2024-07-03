using System.Diagnostics;
using HealthMonitoring.Network.Dtos;
using HealthMonitoring.Network.Endpoints.Telegram;
using HealthMonitoring.Network.HttpContexts.Telegram;
using HealthMonitoring.Options;
using HealthMonitoring.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HealthMonitoring.Services.Implementations;

public class RamMonitor : IRamMonitor
{
    private readonly TelegramSettings _telegramSettings;
    private readonly MonitoringSettings _monitoringSettings;
    private readonly ITelegramHttpClientContext _context;
    private readonly ILogger<RamMonitor> _logger;
    
    
    public RamMonitor(ITelegramHttpClientContext context, IOptionsSnapshot<TelegramSettings> optionsSnapshotTelegram,
        IOptionsSnapshot<MonitoringSettings> optionsSnapshotMonitoring, ILogger<RamMonitor> logger)
    {
        _context = context;
        _logger = logger;
        _telegramSettings = optionsSnapshotTelegram.Value;
        _monitoringSettings = optionsSnapshotMonitoring.Value;
    }

    public async Task Check()
    {
        var freeMemoryPercentage = GetFreeMemoryPercentage();
        if (freeMemoryPercentage < _monitoringSettings.RamMemoryThresholdInPercent)
        {
            var message = $"На ВМ: \'{_monitoringSettings.MachineName}\', процент свободной RAM: \'{freeMemoryPercentage}%\'";
            await SendNotification(message);
        }
    }

    private float GetFreeMemoryPercentage()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "free",
            Arguments = "-m",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        using var reader = process.StandardOutput;
        var output = reader.ReadToEnd();
        var memoryInfo = output.Split('\n')[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var totalMemory = float.Parse(memoryInfo[1]);
        var usedMemory = float.Parse(memoryInfo[2]);
        var freeMemoryPercentage = ((totalMemory - usedMemory) / totalMemory) * 100;
        _logger.LogInformation($"TotalRAMMemory: {totalMemory}, UsedRAMMemory: {usedMemory}, FreeRamMemoryPercent: {freeMemoryPercentage}");
        return freeMemoryPercentage;
    }
    
    private async Task SendNotification(string message)
    {
        _logger.LogInformation($"{message}");
        var dto = new SendNotificationRequest(_telegramSettings.ChatId, message);
        await _context.RunEndpoint(new PostSendNotificationEndpoint(dto));
    }
}