using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDorianBranchBranding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "branches",
                type: "numeric(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "branches",
                type: "numeric(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "branches",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "branches",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO branches ("Id", "Code", "Name", "City", "Address", "PhoneNumber", "OpeningHours", "MapUrl", "Latitude", "Longitude", "IsActive", "CreatedAtUtc")
                VALUES
                    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'CEBOLLAR', 'El Cebollar', 'Cuenca', 'Av. Abelardo J Andrade y Reinaldo Chico Peñaherrera', NULL, NULL, 'https://www.google.com/maps/search/?api=1&query=Av.%20Abelardo%20J%20Andrade%20y%20Reinaldo%20Chico%20Pe%C3%B1aherrera%2C%20Cuenca%2C%20Ecuador', NULL, NULL, TRUE, NOW()),
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'CEMENTERIO', 'Cementerio', 'Cuenca', 'Av. Gonzales Suárez', NULL, NULL, 'https://www.google.com/maps/search/?api=1&query=Av.%20Gonzales%20Su%C3%A1rez%2C%20Cuenca%2C%20Ecuador', NULL, NULL, TRUE, NOW()),
                    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'PARQUEIND', 'Parque Industrial', 'Cuenca', 'Octavio Chacón Moscoso y Cornelio Vintimilla', NULL, NULL, 'https://www.google.com/maps/search/?api=1&query=Octavio%20Chac%C3%B3n%20Moscoso%20y%20Cornelio%20Vintimilla%2C%20Cuenca%2C%20Ecuador', NULL, NULL, TRUE, NOW()),
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'ELTIEMPO', 'El Tiempo', 'Cuenca', 'Av. Loja y Rodrigo de Triana', NULL, NULL, 'https://www.google.com/maps/search/?api=1&query=Av.%20Loja%20y%20Rodrigo%20de%20Triana%2C%20Cuenca%2C%20Ecuador', NULL, NULL, TRUE, NOW()),
                    ('99999999-9999-9999-9999-999999999999', 'AZOGUES', 'Azogues', 'Azogues', 'Calle Simón Bolívar', NULL, NULL, 'https://www.google.com/maps/search/?api=1&query=Calle%20Sim%C3%B3n%20Bol%C3%ADvar%2C%20Azogues%2C%20Ecuador', NULL, NULL, TRUE, NOW())
                ON CONFLICT ("Id") DO UPDATE SET
                    "Code" = EXCLUDED."Code",
                    "Name" = EXCLUDED."Name",
                    "City" = EXCLUDED."City",
                    "Address" = EXCLUDED."Address",
                    "PhoneNumber" = EXCLUDED."PhoneNumber",
                    "OpeningHours" = EXCLUDED."OpeningHours",
                    "MapUrl" = EXCLUDED."MapUrl",
                    "Latitude" = EXCLUDED."Latitude",
                    "Longitude" = EXCLUDED."Longitude",
                    "IsActive" = EXCLUDED."IsActive",
                    "UpdatedAtUtc" = NOW();
                """);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM branches
                WHERE "Id" IN (
                    '99999999-9999-9999-9999-999999999999',
                    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
                    'dddddddd-dddd-dddd-dddd-dddddddddddd',
                    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'
                );
                """);

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "branches");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 20, 14, 26, 122, DateTimeKind.Unspecified).AddTicks(2112), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 20, 14, 26, 122, DateTimeKind.Unspecified).AddTicks(2114), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 20, 14, 26, 122, DateTimeKind.Unspecified).AddTicks(2114), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 20, 14, 26, 122, DateTimeKind.Unspecified).AddTicks(2115), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 20, 14, 26, 122, DateTimeKind.Unspecified).AddTicks(2116), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
