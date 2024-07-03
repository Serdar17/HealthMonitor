namespace HealthMonitoring.Options;

public class TelegramSettings
{
    public const string SectionName = "TelegramBotSettings";
    
    public string Token { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
}