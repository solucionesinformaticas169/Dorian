namespace Dorian.Infrastructure.Authentication;

using Dorian.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Identity;

public sealed class PasswordHasherAdapter : IAppPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();
    public string Hash(string password) => _passwordHasher.HashPassword(new object(), password);
    public bool Verify(string passwordHash, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
