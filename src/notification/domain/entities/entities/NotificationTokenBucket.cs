namespace Notification.Domain.Entites;

public class NotificationTokenBucket
{
    public string? Key { get; set; }
    public long TokensRemaining { get; set; }
}