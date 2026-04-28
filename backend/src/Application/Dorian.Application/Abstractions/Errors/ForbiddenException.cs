namespace Dorian.Application.Abstractions.Errors;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
