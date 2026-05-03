using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBranchSeedsForDorian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 171, DateTimeKind.Unspecified).AddTicks(1170), new TimeSpan(0, 0, 0, 0, 0)), "Lun a Vie 06:00 - 21:00 | Sab 08:00 - 13:00", "0990001005" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 171, DateTimeKind.Unspecified).AddTicks(1150), new TimeSpan(0, 0, 0, 0, 0)), "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", "0990001001" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Address", "Code", "CreatedAtUtc", "MapUrl", "Name", "OpeningHours", "PhoneNumber" },
                values: new object[] { "Av. Gonzales Suárez y Jijón de Caamaño", "GONSUAREZ", new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 171, DateTimeKind.Unspecified).AddTicks(1165), new TimeSpan(0, 0, 0, 0, 0)), "https://www.google.com/maps/search/?api=1&query=Av.%20Gonzales%20Su%C3%A1rez%20y%20Jij%C3%B3n%20de%20Caama%C3%B1o%2C%20Cuenca%2C%20Ecuador", "Gonzáles Suárez", "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", "0990001002" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 171, DateTimeKind.Unspecified).AddTicks(1167), new TimeSpan(0, 0, 0, 0, 0)), "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", "0990001003" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 171, DateTimeKind.Unspecified).AddTicks(1169), new TimeSpan(0, 0, 0, 0, 0)), "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", "0990001004" });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 172, DateTimeKind.Unspecified).AddTicks(9215), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 172, DateTimeKind.Unspecified).AddTicks(9218), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 172, DateTimeKind.Unspecified).AddTicks(9219), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 172, DateTimeKind.Unspecified).AddTicks(9220), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 5, 42, 58, 172, DateTimeKind.Unspecified).AddTicks(9220), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 304, DateTimeKind.Unspecified).AddTicks(9487), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 304, DateTimeKind.Unspecified).AddTicks(9420), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Address", "Code", "CreatedAtUtc", "MapUrl", "Name", "OpeningHours", "PhoneNumber" },
                values: new object[] { "Av. Gonzales Suárez", "CEMENTERIO", new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 304, DateTimeKind.Unspecified).AddTicks(9446), new TimeSpan(0, 0, 0, 0, 0)), "https://www.google.com/maps/search/?api=1&query=Av.%20Gonzales%20Su%C3%A1rez%2C%20Cuenca%2C%20Ecuador", "Cementerio", null, null });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 304, DateTimeKind.Unspecified).AddTicks(9451), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAtUtc", "OpeningHours", "PhoneNumber" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 304, DateTimeKind.Unspecified).AddTicks(9453), new TimeSpan(0, 0, 0, 0, 0)), null, null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 307, DateTimeKind.Unspecified).AddTicks(2962), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 307, DateTimeKind.Unspecified).AddTicks(2966), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 307, DateTimeKind.Unspecified).AddTicks(2967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 307, DateTimeKind.Unspecified).AddTicks(2967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 4, 59, 24, 307, DateTimeKind.Unspecified).AddTicks(2968), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
