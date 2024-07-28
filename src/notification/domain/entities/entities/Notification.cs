
using Notification.Domain.Enums;

namespace Notification.Domain.Entites;

public class Notification : BaseEntity
{
    public required NotificationType Type { get; set; } = NotificationType.Email;
    public required string Sender { get; set; }
    public required string Recipient { get; set; }
    public required string Message { get; set; }
}
