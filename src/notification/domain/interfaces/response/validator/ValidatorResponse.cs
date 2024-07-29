namespace Notification.Domain.Interfaces.Validator;

public class ValidatorResponse
{
    private readonly List<string> _errors = [];
    public ValidatorResponse AddError(string message)
    {
        _errors.Add(message);
        return this;
    }
    public string ErrorMessage
    {
        get
        {
            return string.Join(";", _errors);
        }
    }
    public bool IsValid
    {
        get
        {
            return _errors.Count == 0;
        }
    }

}