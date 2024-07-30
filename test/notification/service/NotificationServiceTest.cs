using Notification.Domain.Entites;
using Notification.Domain.Enums;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Validator;
using Notification.Service.Test.Mocks;

namespace Notification.Service.Test;

[Trait("UnitTest", "NotificationTest")]
public class NotificationServiceTest
{

    [Fact]
    public async void ShouldCallValidateParameterOnce()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new MockNotificationrequestValidator();
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(1, requestValidator.ValidateCalls);
    }


    [Fact]
    public async void ShouldReturnErrorIfInvalidParameters()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        requestValidator.MockResponseResponse.AddError("Sender is Invalid").AddError("Recipient is invalid");
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async void ShouldReturnFailedDependencyIfCacheServiceThrows()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket],
            CanThrowCacheServerOffline = true
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.FailedDependency, result.StatusCode);
    }

    [Fact]
    public async void ShouldReturnNotFoundIfSenderRuleNotFound()
    {
        // Arrange
        MockNotificationrequestValidator requestValidator = new();
        MockCacheService mockCacheService = new()
        {
            MockReturns = [null, null]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("invalid_sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.Notfound, result.StatusCode);
    }

    [Fact]
    public async void ShouldCallCacheServiceTwiceForBothKeys()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(2, mockCacheService.CacheCalls);
        List<string> expectedKeys = new() { $"SenderRule:{notificationRequest.Sender}", $"Bucket:{notificationRequest.Recipient}" };
        Assert.Equal(expectedKeys, mockCacheService.KeysSearched);
    }

    [Fact]
    public async void ShouldReturnTooManyRequestsIfRemainingTokenIsZero()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.TooManyRequest, result.StatusCode);
    }

    [Fact]
    public async void ShouldReturnSuccessIfThereIsRemainingToken()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new()
        {
            Sender = "sender",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 2
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = [senderRule, tokenBucket]
        };
        NotificationService sut = new(requestValidator, mockCacheService);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.True(result.HasSucceed);
        Assert.Equal(NotificationResponseCode.Success, result.StatusCode);
        Assert.True(string.IsNullOrEmpty(result.ErroMessage));
    }

}