namespace Dorian.Application.Abstractions.Auth;

public interface ICurrentUserService
{
    CurrentUser User { get; }
}
