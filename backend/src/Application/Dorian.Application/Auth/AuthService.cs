namespace Dorian.Application.Auth;

using Dorian.Application.Abstractions.Auth;
using Dorian.Application.Abstractions.Errors;
using Dorian.Application.Abstractions.Persistence;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class AuthService : IAuthService
{
    private readonly IDorianDbContext _dbContext;
    private readonly IAppPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenHasher _tokenHasher;

    public AuthService(IDorianDbContext dbContext, IAppPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, ITokenHasher tokenHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _tokenHasher = tokenHasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken))
            throw new ValidationAppException("A user with that email already exists.");

        var branchId = request.BranchId;
        if (branchId.HasValue && !await _dbContext.Branches.AnyAsync(x => x.Id == branchId.Value, cancellationToken))
            throw new NotFoundException("The selected branch does not exist.");

        var customerRoleId = await _dbContext.Roles.Where(x => x.Name == RoleNames.Customer).Select(x => x.Id).SingleAsync(cancellationToken);
        var user = new User(Guid.NewGuid(), normalizedEmail, request.FullName, _passwordHasher.Hash(request.Password));
        user.UpdateProfile(request.FullName, request.PhoneNumber);
        user.AssignToBranch(branchId);
        user.SetRoles([customerRoleId]);
        user.SetActive(true);

        var (firstName, lastName) = SplitName(request.FullName);
        var customer = new Customer(
            Guid.NewGuid(),
            user.Id,
            branchId,
            firstName,
            lastName,
            $"SELF-{user.Id.ToString("N")[..8].ToUpperInvariant()}",
            request.PhoneNumber,
            null,
            Gender.Unspecified,
            null,
            null,
            null,
            CustomerStatus.Active);

        var response = IssueTokens(user, [RoleNames.Customer], ipAddress);
        _dbContext.Users.Add(user);
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).SingleOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password) || !user.IsActive)
            throw new UnauthorizedException("Invalid credentials.");

        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var response = IssueTokens(user, roles, ipAddress);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHasher.Hash(request.RefreshToken);
        var refreshToken = await _dbContext.RefreshTokens.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role).SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (refreshToken is null || !refreshToken.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        var replacement = _jwtTokenService.GenerateRefreshToken();
        var replacementHash = _tokenHasher.Hash(replacement.Token);
        refreshToken.Revoke(replacementHash);

        var user = refreshToken.User;
        var replacementEntity = new RefreshToken(Guid.NewGuid(), user.Id, replacementHash, replacement.ExpiresAtUtc, ipAddress);
        user.AddRefreshToken(replacementEntity);
        _dbContext.RefreshTokens.Add(replacementEntity);

        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken.Token, replacement.Token, accessToken.ExpiresAtUtc, replacement.ExpiresAtUtc, new AuthenticatedUserResponse(user.Id, user.Email, user.FullName, user.BranchId, roles));
    }

    public async Task LogoutAsync(Guid userId, LogoutRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHasher.Hash(request.RefreshToken);
        var refreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == userId && x.TokenHash == tokenHash, cancellationToken);
        if (refreshToken is null || !refreshToken.IsActive) return;
        refreshToken.Revoke();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private AuthResponse IssueTokens(User user, IReadOnlyCollection<string> roles, string ipAddress)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken(Guid.NewGuid(), user.Id, _tokenHasher.Hash(refreshToken.Token), refreshToken.ExpiresAtUtc, ipAddress);
        user.AddRefreshToken(refreshTokenEntity);
        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        return new AuthResponse(accessToken.Token, refreshToken.Token, accessToken.ExpiresAtUtc, refreshToken.ExpiresAtUtc, new AuthenticatedUserResponse(user.Id, user.Email, user.FullName, user.BranchId, roles));
    }

    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        var parts = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            return ("Cliente", "Dorian");
        }

        if (parts.Length == 1)
        {
            return (parts[0], "Dorian");
        }

        return (parts[0], string.Join(' ', parts.Skip(1)));
    }

    private sealed class ValidationAppException : AppException
    {
        public ValidationAppException(string message) : base(message) { }
    }
}
