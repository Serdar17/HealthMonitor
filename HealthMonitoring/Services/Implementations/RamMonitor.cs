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
    private readonly TelegramSettings _settings;
    private readonly ITelegramHttpClientContext _context;
    private const int MemoryThreshold = 20; 
    
    public RamMonitor(ITelegramHttpClientContext context, IOptionsSnapshot<TelegramSettings> optionsSnapshot)
    {
        _context = context;
        _settings = optionsSnapshot.Value;
    }

    public async Task Check()
    {
        var memoryPercentage = GetFreeMemoryPercentage();
        // var memoryPercentage = "12";
        Console.WriteLine(memoryPercentage);
        var message = $"Процент свободной памяти: \'{memoryPercentage}\'";
        await SendNotification(message);
    }

    private static float GetFreeMemoryPercentage()
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
        var lines = output.Split('\n');
        var memoryInfo = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var totalMemory = float.Parse(memoryInfo[1]);
        var usedMemory = float.Parse(memoryInfo[2]);
        var freeMemoryPercentage = ((totalMemory - usedMemory) / totalMemory) * 100;
        return freeMemoryPercentage;
    }
    
    private async Task SendNotification(string message)
    {
        var dto = new SendNotificationRequest(_settings.ChatId, message);
        await _context.RunEndpoint(new PostSendNotificationEndpoint(dto));
    }
}