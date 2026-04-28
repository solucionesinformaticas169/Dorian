namespace Dorian.Application.Abstractions.Auth;

using Dorian.Modules.Identity.Domain.Entities;

public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(User user, IReadOnlyCollection<string> roles);
    RefreshTokenResult GenerateRefreshToken();
}
