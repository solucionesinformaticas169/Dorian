namespace Dorian.Application.Abstractions.Auth;

public interface ITokenHasher
{
    string Hash(string value);
}
