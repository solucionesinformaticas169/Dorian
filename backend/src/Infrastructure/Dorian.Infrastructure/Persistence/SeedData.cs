namespace Dorian.Infrastructure.Persistence;

using Dorian.Modules.Identity.Domain.Constants;

public static class SeedData
{
    public static readonly Guid ElCebollarBranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid GonzalesSuarezBranchId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid CementerioBranchId = GonzalesSuarezBranchId;
    public static readonly Guid ParqueIndustrialBranchId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid ElTiempoBranchId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid AzoguesBranchId = Guid.Parse("99999999-9999-9999-9999-999999999999");

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

    public static class BranchMaps
    {
        public const string ElCebollar = "https://www.google.com/maps/search/?api=1&query=Av.%20Abelardo%20J%20Andrade%20y%20Reinaldo%20Chico%20Pe%C3%B1aherrera%2C%20Cuenca%2C%20Ecuador";
        public const string GonzalesSuarez = "https://www.google.com/maps/search/?api=1&query=Av.%20Gonzales%20Su%C3%A1rez%20y%20Jij%C3%B3n%20de%20Caama%C3%B1o%2C%20Cuenca%2C%20Ecuador";
        public const string Cementerio = GonzalesSuarez;
        public const string ParqueIndustrial = "https://www.google.com/maps/search/?api=1&query=Octavio%20Chac%C3%B3n%20Moscoso%20y%20Cornelio%20Vintimilla%2C%20Cuenca%2C%20Ecuador";
        public const string ElTiempo = "https://www.google.com/maps/search/?api=1&query=Av.%20Loja%20y%20Rodrigo%20de%20Triana%2C%20Cuenca%2C%20Ecuador";
        public const string Azogues = "https://www.google.com/maps/search/?api=1&query=Calle%20Sim%C3%B3n%20Bol%C3%ADvar%2C%20Azogues%2C%20Ecuador";
    }
}
