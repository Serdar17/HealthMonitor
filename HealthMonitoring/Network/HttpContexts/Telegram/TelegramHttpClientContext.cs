using Arbus.Network;
using HealthMonitoring.Options;
using Microsoft.Extensions.Options;

namespace HealthMonitoring.Network.HttpContexts.Telegram;

public class TelegramHttpClientContext(INativeHttpClient httpClientHandler, IOptionsSnapshot<TelegramSettings> optionsSnapshot) 
    : HttpClientContext(httpClientHandler), ITelegramHttpClientContext
{
    private readonly string _url = $"https://api.telegram.org/bot{optionsSnapshot.Value.Token}";

    public override Uri GetBaseUri() => new(_url);
}