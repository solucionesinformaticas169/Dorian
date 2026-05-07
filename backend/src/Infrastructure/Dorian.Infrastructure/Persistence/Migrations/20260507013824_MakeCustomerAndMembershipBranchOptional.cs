using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeCustomerAndMembershipBranchOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "memberships",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "customers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Calle Simon Bolivar", new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4833), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Av. Abelardo J Andrade y Reinaldo Chico Penaherrera", new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4805), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Address", "CreatedAtUtc", "Name" },
                values: new object[] { "Av. Gonzales Suarez y Jijon de Caamano", new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4825), new TimeSpan(0, 0, 0, 0, 0)), "Gonzales Suarez" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Octavio Chacon Moscoso y Cornelio Vintimilla", new DateTimeOffset(new DateTime(2026, 5, 7, 1, 38, 23, 955, DateTimeKind.Unspecified).AddTicks(4826), new TimeSpan(0, 0, 0, 0, 0)) });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "memberships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Calle Simón Bolívar", new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5707), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Av. Abelardo J Andrade y Reinaldo Chico Peñaherrera", new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5670), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Address", "CreatedAtUtc", "Name" },
                values: new object[] { "Av. Gonzales Suárez y Jijón de Caamaño", new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5697), new TimeSpan(0, 0, 0, 0, 0)), "Gonzáles Suárez" });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "Address", "CreatedAtUtc" },
                values: new object[] { "Octavio Chacón Moscoso y Cornelio Vintimilla", new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5700), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5703), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2004), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2015), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2019), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2021), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2024), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2026), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2028), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2029), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2030), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2032), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2034), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2035), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2036), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2037), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2039), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2041), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2045), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2077), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2079), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2080), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2082), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2083), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2085), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2086), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2087), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2088), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2090), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2092), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2093), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2094), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2096), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2097), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2098), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2100), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2101), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2103), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2104), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2105), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2106), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2107), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2112), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 30, DateTimeKind.Unspecified).AddTicks(2113), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 31, DateTimeKind.Unspecified).AddTicks(1572), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 31, DateTimeKind.Unspecified).AddTicks(1574), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 31, DateTimeKind.Unspecified).AddTicks(1575), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 31, DateTimeKind.Unspecified).AddTicks(1576), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 31, DateTimeKind.Unspecified).AddTicks(1577), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
