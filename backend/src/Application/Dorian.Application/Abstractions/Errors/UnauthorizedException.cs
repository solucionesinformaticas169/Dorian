namespace Dorian.Application.Abstractions.Errors;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
