namespace Dorian.Application.Abstractions.Errors;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
