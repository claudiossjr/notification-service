namespace Notification.Domain.Exceptions.Parser;

public class ExpirationExpressionNotValidException : Exception
{
    public ExpirationExpressionNotValidException(string expression) : base($"Expression is not valid [{expression ?? ""}]")
    {
    }
}