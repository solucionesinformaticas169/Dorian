namespace Dorian.Application.Abstractions.Errors;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}
