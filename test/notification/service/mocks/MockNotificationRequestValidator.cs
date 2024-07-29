using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Validator;

namespace Notification.Service.Test.Mocks;

public class MockNotificationrequestValidator : INotificationRequestValidator
{
    public ValidatorResponse MockResponseResponse { get; set; } = new();
    public int ValidateCalls { get; private set; } = 0;

    public ValidatorResponse Validate(NotificationRequest request)
    {
        ValidateCalls++;
        return MockResponseResponse;
    }
}