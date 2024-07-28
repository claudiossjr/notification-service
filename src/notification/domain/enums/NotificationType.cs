using System.ComponentModel;

namespace Notification.Domain.Enums;

public enum NotificationType
{
    [Description("Console Notification")]
    Console = 0,
    [Description("Email Notification")]
    Email = 1,
    [Description("SMS Notification")]
    SMS = 2,
    [Description("Push Notification")]
    PushNotification = 3
}