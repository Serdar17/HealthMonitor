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
    private readonly TelegramSettings _settings;
    private const int MemoryThreshold = 5; 

    public RomMonitor(ITelegramHttpClientContext context, IOptionsSnapshot<TelegramSettings> optionsSnapshot)
    {
        _context = context;
        _settings = optionsSnapshot.Value;
        
    }

    public async Task Check()
    {
        var memoryPercentage = GetFreeMemoryPercentage();
        Console.WriteLine(memoryPercentage);
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
        var dto = new SendNotificationRequest(_settings.ChatId, message);
        await _context.RunEndpoint(new PostSendNotificationEndpoint(dto));
    }
}