using Notification.Domain.Enums;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Validator;
using Notification.Service.Test.Mocks;

namespace Notification.Service.Test;

public class NotificationServiceTest
{

    [Fact]
    [Trait("UnitTest", "NotificationTest")]
    public void ShouldReturnErrorIfInvalidParameters()
    {
        // Arrange
        INotificationRequestValidator requestValidator = new MockNotificationrequestValidator();
        NotificationService sut = new(requestValidator);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.BadRequest, result.StatusCode);
    }

}