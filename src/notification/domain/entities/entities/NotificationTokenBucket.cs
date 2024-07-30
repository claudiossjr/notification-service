namespace Notification.Domain.Entites;

public class NotificationTokenBucket
{
    public required string Key { get; set; }
    public long TokensRemaining { get; set; }
}