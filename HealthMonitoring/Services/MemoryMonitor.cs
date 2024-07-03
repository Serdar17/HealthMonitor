using HealthMonitoring.Network.Dtos;
using HealthMonitoring.Network.Endpoints.Telegram;
using HealthMonitoring.Network.HttpContexts.Telegram;
using HealthMonitoring.Options;
using HealthMonitoring.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HealthMonitoring.Services;

public abstract class MemoryMonitor(
    IOptionsSnapshot<TelegramSettings> optionsSnapshotTelegram,
    ITelegramHttpClientContext context,
    ILogger<MemoryMonitor> logger) : IRamMonitor, IRomMonitor
{
    public virtual Task CheckRam()
    {
        return Task.CompletedTask;
    }

    public virtual Task CheckRom()
    {
        return Task.CompletedTask;
    }

    protected async Task SendNotification(string message)
    {
        logger.LogInformation($"{message}");
        var dto = new SendNotificationRequest(optionsSnapshotTelegram.Value.ChatId, message);
        await context.RunEndpoint(new PostSendNotificationEndpoint(dto));
    }
}