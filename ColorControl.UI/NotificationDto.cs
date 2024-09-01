using NStandard;

namespace ColorControl.UI;

public class NotificationDto
{
    public long Id { get; }
    public string Message { get; }
    public string Level { get; } = Constants.Info;
    public DateTime? ScheduledAt { get; set; }
    public string? InternalUrl { get; set; }
    public DateTime? ShownAt { get; set; }

    public NotificationDto(string message, string level = Constants.Info)
    {
        Message = message;
        Level = level;
        Id = Random.Shared.NextInt64();
    }

    public string Category => Level switch
    {
        Constants.Danger => "Error",
        _ => Level[0].ToUpper() + Level.Substring(1)
    };
}
