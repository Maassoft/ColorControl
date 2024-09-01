namespace ColorControl.UI.Services;

public class NotificationService
{
    public event Func<NotificationDto, Task>? OnNotification;

    private List<NotificationDto> _scheduledNotifications = new();

    public void SendNotification(NotificationDto notification)
    {
        var now = DateTime.UtcNow;
        if (notification.ScheduledAt == null || notification.ScheduledAt <= now)
        {
            OnNotification?.Invoke(notification);

            return;
        }

        Task.Run(async () =>
        {
            await Task.Delay(notification.ScheduledAt.Value - now);

            OnNotification?.Invoke(notification);
        });
    }
}