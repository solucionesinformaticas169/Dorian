namespace Dorian.Application.Abstractions.Auth;

public sealed record CurrentUser(
    Guid? UserId,
    Guid? BranchId,
    IReadOnlyCollection<string> Roles)
{
    public bool IsAuthenticated => UserId.HasValue;

    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
}
