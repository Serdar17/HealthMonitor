using System.Diagnostics;
using HealthMonitoring.Network.Dtos;
using HealthMonitoring.Network.Endpoints.Telegram;
using HealthMonitoring.Network.HttpContexts.Telegram;
using HealthMonitoring.Options;
using HealthMonitoring.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HealthMonitoring.Services.Implementations;

public class RomMonitor : IRomMonitor
{
    private readonly ITelegramHttpClientContext _context;
    private readonly TelegramSettings _telegramSettings;
    private readonly MonitoringSettings _monitoringSettings;
    private readonly ILogger<RomMonitor> _logger;

    public RomMonitor(ITelegramHttpClientContext context, IOptionsSnapshot<TelegramSettings> optionsSnapshotTelegram,
        IOptionsSnapshot<MonitoringSettings> optionsSnapshotMonitoring, ILogger<RomMonitor> logger)
    {
        _context = context;
        _logger = logger;
        _telegramSettings = optionsSnapshotTelegram.Value;
        _monitoringSettings = optionsSnapshotMonitoring.Value;
    }

    public async Task Check()
    {
        var memoryPercentage = GetFreeMemoryPercentage();
        var message = $"Процент свободной постоянной памяти: \'{memoryPercentage}\'";
        await SendNotification(message);
    }

    private float GetFreeMemoryPercentage()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "df",
            Arguments = "-h /",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        using var reader = process.StandardOutput;
        var output = reader.ReadToEnd();
        var percentUsed = output.Split('\n')[1].Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[4];
        percentUsed = percentUsed.TrimEnd('%');
        var usedDiskPercentage = float.Parse(percentUsed);
        var freeDiskPercentage = 100 - usedDiskPercentage;
        return freeDiskPercentage;
    }

    private async Task SendNotification(string message)
    {
        _logger.LogInformation($"{message}");
        var dto = new SendNotificationRequest(_telegramSettings.ChatId, message);
        await _context.RunEndpoint(new PostSendNotificationEndpoint(dto));
    }
}