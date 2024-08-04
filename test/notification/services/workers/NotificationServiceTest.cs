using System.Text.Json;
using Common.Helpers;
using Notification.Domain.Entites;
using Notification.Domain.Enums;
using Notification.Domain.Interfaces.Request;
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
        requestValidator.MockResponseResponse.AddError("Sender is Invalid").AddError("Recipient is invalid");
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), tokenBucket}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

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
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), tokenBucket}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

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
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), tokenBucket}
            },
            CanThrowCacheServerOffline = true
        };
        NotificationService sut = new(requestValidator, mockCacheService);

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
        NotificationRequest notificationRequest = new("invalid_sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), null},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), null}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.Notfound, result.StatusCode);
    }

    [Fact]
    public async void ShouldCallCacheServiceTwiceForBothKeys()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), null}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(2, mockCacheService.CacheFindCalls);
        Assert.Equal(1, mockCacheService.CacheDecreaseCalls);
        Assert.Equal(1, mockCacheService.CacheCreateCalls);
        Assert.Equal("0", mockCacheService.ValueCreated);
        List<string> expectedKeys = new() { CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient) };
        Assert.Equal(expectedKeys, mockCacheService.KeysSearched);
    }

    [Fact]
    public async void ShouldReturnTooManyRequestsIfRemainingTokenIsZero()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = senderRule.RateLimit + 1
        };
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), $"{tokenBucket.TokensRemaining}"}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.Equal(NotificationResponseCode.TooManyRequest, result.StatusCode);
    }

    [Fact]
    public async void ShouldReturnSuccessIfThereIsRemainingToken()
    {
        MockNotificationrequestValidator requestValidator = new();
        NotificationRule senderRule = new("sender", 2, 10);
        NotificationTokenBucket tokenBucket = new()
        {
            Key = "recipient",
            TokensRemaining = 0
        };
        NotificationRequest notificationRequest = new("sender", "recipient", "message");
        MockCacheService mockCacheService = new()
        {
            MockReturns = new()
            {
                {CacheKeyNameHelper.GetNotificationRuleKey(notificationRequest.Sender), senderRule},
                {CacheKeyNameHelper.GetNotificationTokenBucketKey(notificationRequest.Sender, notificationRequest.Recipient), $"{tokenBucket.TokensRemaining}"}
            }
        };
        NotificationService sut = new(requestValidator, mockCacheService);

        // Act
        var result = await sut.Notify(notificationRequest);

        // Assert
        Assert.True(result.HasSucceed);
        Assert.Equal(NotificationResponseCode.Success, result.StatusCode);
        Assert.Equal($"Notification to {notificationRequest.Recipient} from {notificationRequest.Sender} was sent!", result.Message);
    }

}