using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "meal_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    DailyCaloriesTarget = table.Column<int>(type: "integer", nullable: false),
                    ProteinGrams = table.Column<int>(type: "integer", nullable: false),
                    CarbsGrams = table.Column<int>(type: "integer", nullable: false),
                    FatGrams = table.Column<int>(type: "integer", nullable: false),
                    MealsPerDay = table.Column<int>(type: "integer", nullable: false),
                    WaterLitersTarget = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DietaryRestrictions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "meal_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MealPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Calories = table.Column<int>(type: "integer", nullable: false),
                    ProteinGrams = table.Column<int>(type: "integer", nullable: false),
                    CarbsGrams = table.Column<int>(type: "integer", nullable: false),
                    FatGrams = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_meal_items_meal_plans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalTable: "meal_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5707), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5670), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5697), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 20, 49, 28, 28, DateTimeKind.Unspecified).AddTicks(5700), new TimeSpan(0, 0, 0, 0, 0)));

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

            migrationBuilder.CreateIndex(
                name: "IX_meal_items_MealPlanId",
                table: "meal_items",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_meal_plans_CustomerId",
                table: "meal_plans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_meal_plans_DayOfWeek",
                table: "meal_plans",
                column: "DayOfWeek");

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_profiles_CustomerId",
                table: "nutrition_profiles",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meal_items");

            migrationBuilder.DropTable(
                name: "nutrition_profiles");

            migrationBuilder.DropTable(
                name: "meal_plans");

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 119, DateTimeKind.Unspecified).AddTicks(4614), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 119, DateTimeKind.Unspecified).AddTicks(4562), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 119, DateTimeKind.Unspecified).AddTicks(4595), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 119, DateTimeKind.Unspecified).AddTicks(4605), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 119, DateTimeKind.Unspecified).AddTicks(4611), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6924), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6938), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6942), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6948), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6950), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6953), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6986), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6990), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6992), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6995), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6997), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(6999), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7001), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7003), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7005), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7006), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7008), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7011), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7031), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7033), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7036), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7038), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7040), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7042), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7050), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7052), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7053), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7055), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7057), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7065), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7067), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7069), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7070), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7073), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7074), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7076), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7078), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7079), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7080), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7082), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7083), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 122, DateTimeKind.Unspecified).AddTicks(7085), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 123, DateTimeKind.Unspecified).AddTicks(6956), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 123, DateTimeKind.Unspecified).AddTicks(6959), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 123, DateTimeKind.Unspecified).AddTicks(6960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 123, DateTimeKind.Unspecified).AddTicks(6961), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 18, 0, 20, 123, DateTimeKind.Unspecified).AddTicks(6962), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
