using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBranchMapLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MapUrl",
                table: "branches",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 847, DateTimeKind.Unspecified).AddTicks(8244), new TimeSpan(0, 0, 0, 0, 0)), -2.7403717m, -78.8459882m, "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.7403562,-78.8460382,3a,75y,32h,95.19t/data=!3m7!1e1!3m5!1sEe0Zo4I_g6dPBVDyhGfVIQ!2e0!6shttps:%2F%2Fstreetviewpixels-pa.googleapis.com%2Fv1%2Fthumbnail%3Fcb_client%3Dmaps_sv.tactile%26w%3D900%26h%3D600%26pitch%3D-5.187052136132152%26panoid%3DEe0Zo4I_g6dPBVDyhGfVIQ%26yaw%3D32.00196772697777!7i13312!8i6656!4m14!1m7!3m6!1s0x91cd12a4782b1377:0x78cc04740cd29b87!2sGimnasio+Dorian!8m2!3d-2.7403717!4d-78.8459882!16s%2Fg%2F11c1sjv77x!3m5!1s0x91cd12a4782b1377:0x78cc04740cd29b87!8m2!3d-2.7403717!4d-78.8459882!16s%2Fg%2F11c1sjv77x?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 847, DateTimeKind.Unspecified).AddTicks(8219), new TimeSpan(0, 0, 0, 0, 0)), -2.8852267m, -79.0142288m, "https://www.google.com/maps/place/Gimnasio+Dorian+El+Cebollar/@-2.8847735,-79.0148627,308m/data=!3m1!1e3!4m15!1m8!3m7!1s0x91cd1801ee836d55:0xdde7783314f7543!2sAvenida+Abelardo+J.+Andrade+%26+Reinaldo+Chico+Pe%C3%B1aherrera,+Cuenca!3b1!8m2!3d-2.8848158!4d-79.0144933!16s%2Fg%2F11hbgj3_l3!3m5!1s0x91cd190f36e8fffd:0x85ddea081eff38fd!8m2!3d-2.8852267!4d-79.0142288!16s%2Fg%2F11pcmxz6xg?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 847, DateTimeKind.Unspecified).AddTicks(8235), new TimeSpan(0, 0, 0, 0, 0)), -2.8996747m, -78.9895041m, "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.8995393,-78.9897921,98m/data=!3m1!1e3!4m9!1m2!2m1!1sAv.+Gonzales+Su%C3%A1rez+y+Jij%C3%B3n+de+Caama%C3%B1o,+Cuenca,+Ecuador!3m5!1s0x91cd183915090e6b:0xc65141231bf2288e!8m2!3d-2.8996747!4d-78.9895041!16s%2Fg%2F11gxss0lqq?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 847, DateTimeKind.Unspecified).AddTicks(8240), new TimeSpan(0, 0, 0, 0, 0)), -2.8777373m, -78.9793991m, "https://www.google.com/maps/place/Gimnasio+Dorian/@-2.8775144,-78.979727,179m/data=!3m1!1e3!4m15!1m8!3m7!1s0x91cd17838b61046b:0xa44b4e4dd69ac4c9!2sOctavio+Chacon+Moscoso+%26+Cornelio+Vintimilla,+Cuenca!3b1!8m2!3d-2.8773221!4d-78.9794501!16s%2Fg%2F11gf3r_2bj!3m5!1s0x91cd17f4c9a163f5:0xf57fe50d67d3ba5e!8m2!3d-2.8777373!4d-78.9793991!16s%2Fg%2F11sw020yvb?entry=ttu&g_ep=EgoyMDI2MDUwMi4wIKXMDSoASAFQAw%3D%3D" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 847, DateTimeKind.Unspecified).AddTicks(8243), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3856), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3875), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3881), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3889), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3895), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3900), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3904), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3909), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3912), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3918), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3925), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3926), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3934), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3936), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3939), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3940), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3943), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3945), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3946), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3949), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3950), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3951), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3953), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3979), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3980), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3983), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3985), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3987), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3989), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(3992), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4001), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4007), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4022), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4025), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4026), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4027), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4028), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4034), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4035), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4037), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 850, DateTimeKind.Unspecified).AddTicks(4038), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 851, DateTimeKind.Unspecified).AddTicks(7015), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 851, DateTimeKind.Unspecified).AddTicks(7018), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 851, DateTimeKind.Unspecified).AddTicks(7019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 851, DateTimeKind.Unspecified).AddTicks(7019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 6, 26, 33, 851, DateTimeKind.Unspecified).AddTicks(7020), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MapUrl",
                table: "branches",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4833), new TimeSpan(0, 0, 0, 0, 0)), null, null, "https://www.google.com/maps/search/?api=1&query=Calle%20Sim%C3%B3n%20Bol%C3%ADvar%2C%20Azogues%2C%20Ecuador" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4805), new TimeSpan(0, 0, 0, 0, 0)), null, null, "https://www.google.com/maps/search/?api=1&query=Av.%20Abelardo%20J%20Andrade%20y%20Reinaldo%20Chico%20Pe%C3%B1aherrera%2C%20Cuenca%2C%20Ecuador" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4825), new TimeSpan(0, 0, 0, 0, 0)), null, null, "https://www.google.com/maps/search/?api=1&query=Av.%20Gonzales%20Su%C3%A1rez%20y%20Jij%C3%B3n%20de%20Caama%C3%B1o%2C%20Cuenca%2C%20Ecuador" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAtUtc", "Latitude", "Longitude", "MapUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4826), new TimeSpan(0, 0, 0, 0, 0)), null, null, "https://www.google.com/maps/search/?api=1&query=Octavio%20Chac%C3%B3n%20Moscoso%20y%20Cornelio%20Vintimilla%2C%20Cuenca%2C%20Ecuador" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4830), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3378), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3386), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3389), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3390), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3392), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3394), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3396), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3398), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3399), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3401), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3402), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3403), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3405), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3406), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3407), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3408), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3409), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3411), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3412), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3414), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3416), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3417), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3418), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3419), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3420), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3422), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3423), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3424), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3425), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3426), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3502), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3504), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3505), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3509), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3511), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3512), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3514), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3515), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3516), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3517), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3518), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 957, DateTimeKind.Unspecified).AddTicks(3519), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 958, DateTimeKind.Unspecified).AddTicks(2114), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 958, DateTimeKind.Unspecified).AddTicks(2116), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 958, DateTimeKind.Unspecified).AddTicks(2117), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 958, DateTimeKind.Unspecified).AddTicks(2118), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 958, DateTimeKind.Unspecified).AddTicks(2118), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
