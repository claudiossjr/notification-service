namespace Notification.Domain.Interfaces.Validator;

public class ValidatorResponse
{
    private readonly List<string> _errors = new();
    public void AddError(string message)
    {
        _errors.Add(message);
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