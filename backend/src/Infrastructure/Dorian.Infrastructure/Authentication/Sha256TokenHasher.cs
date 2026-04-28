namespace Dorian.Infrastructure.Authentication;

using System.Security.Cryptography;
using System.Text;
using Dorian.Application.Abstractions.Auth;

public sealed class Sha256TokenHasher : ITokenHasher
{
    public string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
