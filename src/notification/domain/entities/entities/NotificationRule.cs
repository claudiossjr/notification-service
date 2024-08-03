namespace Notification.Domain.Entites;

public class NotificationRule
{
    public required string Sender { get; set; }
    public required long RateLimit { get; set; }
    public required long ExpiresInMilliseconds { get; set; }

}