using System.Reflection;
using Arbus.Network;
using HealthMonitoring.Network.HttpContexts.Telegram;

namespace HealthMonitoring.Configuration;

public static class ServiceConfiguration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services
            .AddTransient<INetworkManager, WindowsNetworkManager>()
            .AddSingleton<INativeHttpClient>(x =>
                new WindowsHttpClient(x.GetRequiredService<INetworkManager>(),
                    new(nameof(HealthMonitoring), Assembly.GetExecutingAssembly().GetName().Version?.ToString())))
            .AddScoped<ITelegramHttpClientContext, TelegramHttpClientContext>();
        
        return services;
    }
}