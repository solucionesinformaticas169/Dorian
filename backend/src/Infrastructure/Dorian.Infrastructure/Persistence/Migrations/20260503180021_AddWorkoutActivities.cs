using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workout_activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingDayId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    CaloriesEstimated = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workout_activities_training_days_TrainingDayId",
                        column: x => x.TrainingDayId,
                        principalTable: "training_days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercise_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    MuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_exercise_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workout_exercise_logs_workout_activities_WorkoutActivityId",
                        column: x => x.WorkoutActivityId,
                        principalTable: "workout_activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_workout_activities_CompletedAt",
                table: "workout_activities",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_workout_activities_CustomerId",
                table: "workout_activities",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_workout_activities_TrainingDayId",
                table: "workout_activities",
                column: "TrainingDayId");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercise_logs_MuscleGroup",
                table: "workout_exercise_logs",
                column: "MuscleGroup");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercise_logs_WorkoutActivityId",
                table: "workout_exercise_logs",
                column: "WorkoutActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workout_exercise_logs");

            migrationBuilder.DropTable(
                name: "workout_activities");

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 161, DateTimeKind.Unspecified).AddTicks(9429), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 161, DateTimeKind.Unspecified).AddTicks(9408), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 161, DateTimeKind.Unspecified).AddTicks(9424), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 161, DateTimeKind.Unspecified).AddTicks(9426), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 161, DateTimeKind.Unspecified).AddTicks(9427), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6140), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6153), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6154), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6156), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6159), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6160), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6162), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6163), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6165), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6167), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6168), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6170), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6171), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6172), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6174), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6175), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6177), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6178), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6179), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6180), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6181), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6182), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6183), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6184), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6185), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6187), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6190), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6192), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6193), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6194), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6195), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6196), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6198), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6199), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6200), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6201), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6202), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6203), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6205), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6206), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "exercise_catalog",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6207), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 164, DateTimeKind.Unspecified).AddTicks(494), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 164, DateTimeKind.Unspecified).AddTicks(497), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 164, DateTimeKind.Unspecified).AddTicks(497), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 164, DateTimeKind.Unspecified).AddTicks(498), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 164, DateTimeKind.Unspecified).AddTicks(499), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
