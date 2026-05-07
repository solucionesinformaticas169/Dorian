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
        public const string ElCebollar = "https://www.google.com/maps/place/Gimnasio+Dorian+El+Cebollar/@-2.8847735,-79.0148627,308m/data=!3m1!1e3!4m15!1m8!3m7!1s0x91cd1801ee836d55:0xdde7783314f7543!2sAvenida+Abelardo+J.+Andrade+%26+Reinaldo+Chico+Pe%C3%B1aherrera,+Cuenca!3b1!8m2!3d-2.8848158!4d-79.0144933!16s%2Fg%2F11hbgj3_l3!3m5!1s0x91cd190f36e8fffd:0x85ddea081eff38fd!8m2!3d-2.8852267!4d-79.0142288!16s%2Fg%2F11pcmxz6xg?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D";
        public const string GonzalesSuarez = "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.8995393,-78.9897921,98m/data=!3m1!1e3!4m9!1m2!2m1!1sAv.+Gonzales+Su%C3%A1rez+y+Jij%C3%B3n+de+Caama%C3%B1o,+Cuenca,+Ecuador!3m5!1s0x91cd183915090e6b:0xc65141231bf2288e!8m2!3d-2.8996747!4d-78.9895041!16s%2Fg%2F11gxss0lqq?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D";
        public const string Cementerio = GonzalesSuarez;
        public const string ParqueIndustrial = "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.8775144,-78.979727,179m/data=!3m1!1e3!4m15!1m8!3m7!1s0x91cd17838b61046b:0xa44b4e4dd69ac4c9!2sOctavio+Chacon+Moscoso+%26+Cornelio+Vintimilla,+Cuenca!3b1!8m2!3d-2.8773221!4d-78.9794501!16s%2Fg%2F11gf3r_2bj!3m5!1s0x91cd17f4c9a163f5:0xf57fe50d67d3ba5e!8m2!3d-2.8777373!4d-78.9793991!16s%2Fg%2F11sw020yvb?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D";
        public const string ElTiempo = "https://www.google.com/maps/search/?api=1&query=Av.%20Loja%20y%20Rodrigo%20de%20Triana%2C%20Cuenca%2C%20Ecuador";
        public const string Azogues = "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.7403562,-78.8460382,3a,75y,32h,95.19t/data=!3m7!1e1!3m5!1sEe0Zo4I_g6dPBVDyhGfVIQ!2e0!6shttps:%2F%2Fstreetviewpixels-pa.googleapis.com%2Fv1%2Fthumbnail%3Fcb_client%3Dmaps_sv.tactile%26w%3D900%26h%3D600%26pitch%3D-5.187052136132152%26panoid%3DEe0Zo4I_g6dPBVDyhGfVIQ%26yaw%3D32.00196772697777!7i13312!8i6656!4m14!1m7!3m6!1s0x91cd12a4782b1377:0x78cc04740cd29b87!2sGimnasio+Dorian!8m2!3d-2.7403717!4d-78.8459882!16s%2Fg%2F11c1sjv77x!3m5!1s0x91cd12a4782b1377:0x78cc04740cd29b87!8m2!3d-2.7403717!4d-78.8459882!16s%2Fg%2F11c1sjv77x?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D";
    }
}
