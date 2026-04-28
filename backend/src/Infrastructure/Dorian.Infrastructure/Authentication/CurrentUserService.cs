namespace Dorian.Infrastructure.Authentication;

using System.Security.Claims;
using Dorian.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public CurrentUser User
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
                return new CurrentUser(null, null, []);

            Guid? userId = Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedUserId) ? parsedUserId : null;
            Guid? branchId = Guid.TryParse(principal.FindFirstValue("branch_id"), out var parsedBranchId) ? parsedBranchId : null;
            var roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
            return new CurrentUser(userId, branchId, roles);
        }
    }
}
