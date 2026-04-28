namespace Dorian.Application.Abstractions.Auth;

public interface IAppPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}
