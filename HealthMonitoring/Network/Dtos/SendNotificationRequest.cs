namespace HealthMonitoring.Network.Dtos;

public record SendNotificationRequest(string chat_id, string text);