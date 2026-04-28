namespace Dorian.Infrastructure.Persistence;

using Dorian.Modules.Identity.Domain.Constants;

public static class SeedData
{
    public static readonly Guid CustomerRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TrainerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ReceptionRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid BranchAdminRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid SuperAdminRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly IReadOnlyDictionary<string, Guid> Roles = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
    {
        [RoleNames.Customer] = CustomerRoleId,
        [RoleNames.Trainer] = TrainerRoleId,
        [RoleNames.Reception] = ReceptionRoleId,
        [RoleNames.BranchAdmin] = BranchAdminRoleId,
        [RoleNames.SuperAdmin] = SuperAdminRoleId
    };
}
