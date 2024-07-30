namespace Notification.Domain.Entites;

public class NotificationRule
{
    public required string Sender { get; set; }
    public long RateLimit { get; set; }
    public long TimeSpanInSeconds { get; set; }

}