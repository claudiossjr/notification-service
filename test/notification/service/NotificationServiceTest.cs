using Notification.Domain.Enums;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Validator;
using Notification.Service.Test.Mocks;

namespace Notification.Service.Test;

[Trait("UnitTest", "NotificationTest")]
public class NotificationServiceTest
{

    [Fact]
    public void ShouldCallValidateParameterOnce()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new MockNotificationrequestValidator();
        MockCacheService mockCacheService = new();
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(1, requestValidator.ValidateCalls);
    }


    [Fact]
    public void ShouldReturnErrorIfInvalidParameters()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        requestValidator.MockResponseResponse.AddError("Sender is Invalid").AddError("Recipient is invalid");
        MockCacheService mockCacheService = new();
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public void ShouldReturnFailedDependencyIfCacheServiceThrows()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        MockCacheService mockCacheService = new();
        mockCacheService.CanThrowCacheServerOffline = true;
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.FailedDependency, result.StatusCode);
    }

    [Fact]
    public void ShouldReturnNotFoundIfSenderRuleNotFound()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        MockCacheService mockCacheService = new();
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("invalid_sender", "recipient", "message");

        // Act
        var result = sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.Notfound, result.StatusCode);
    }

    [Fact]
    public void ShouldCallCacheServiceTwiceForBothKeys()
    {
        MockNotificationrequestValidator requestValidator = new();
        MockCacheService mockCacheService = new();
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(2, mockCacheService.CacheCalls);
        List<string> expectedKeys = new() { $"Rule:{notificationRequest.Sender}", $"Bucket:{notificationRequest.Recipient}" };
        Assert.Equal(expectedKeys, mockCacheService.KeysSearched);
    }



}