using Arbus.Network;
using HealthMonitoring.Network.Dtos;

namespace HealthMonitoring.Network.Endpoints.Telegram;

public class PostSendNotificationEndpoint(SendNotificationRequest request) : ApiEndpoint(HttpMethod.Post, "sendMessage")
{
    public override HttpContent? CreateContent() => ToJson(request);
}