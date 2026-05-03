using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    MuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    Equipment = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    VideoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_catalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "training_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    ExperienceLevel = table.Column<int>(type: "integer", nullable: false),
                    FocusMuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_plans_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_phases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DurationWeeks = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_phases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_phases_training_plans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "training_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_weeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingPhaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_weeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_weeks_training_phases_TrainingPhaseId",
                        column: x => x.TrainingPhaseId,
                        principalTable: "training_phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_days",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "integer", nullable: false),
                    Intensity = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_days_training_weeks_TrainingWeekId",
                        column: x => x.TrainingWeekId,
                        principalTable: "training_weeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    MuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    RestSeconds = table.Column<int>(type: "integer", nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_exercises_exercise_catalog_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercise_catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_training_exercises_training_days_TrainingDayId",
                        column: x => x.TrainingDayId,
                        principalTable: "training_days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "exercise_catalog",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedByUserId", "Description", "Difficulty", "Equipment", "ImageUrl", "MuscleGroup", "Name", "Slug", "UpdatedAtUtc", "UpdatedByUserId", "VideoUrl" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6140), new TimeSpan(0, 0, 0, 0, 0)), null, "Ejercicio base para desarrollar fuerza y masa en el pecho.", 2, 3, null, 1, "Press de banca plano", "press-banca-plano", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)), null, "Activa la porcion superior del pecho con recorrido controlado.", 1, 2, null, 1, "Press inclinado con mancuernas", "press-inclinado-mancuernas", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6153), new TimeSpan(0, 0, 0, 0, 0)), null, "Aislamiento para estirar y contraer el pecho.", 1, 2, null, 1, "Aperturas con mancuernas", "aperturas-mancuernas", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6154), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimiento multiarticular para pecho y triceps.", 3, 1, null, 1, "Fondos en paralelas", "fondos-paralelas", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6156), new TimeSpan(0, 0, 0, 0, 0)), null, "Mejora la amplitud dorsal y la tecnica de traccion.", 1, 5, null, 2, "Jalon al pecho", "jalon-pecho", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6159), new TimeSpan(0, 0, 0, 0, 0)), null, "Desarrolla espalda media y control postural.", 2, 3, null, 2, "Remo con barra", "remo-barra", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6160), new TimeSpan(0, 0, 0, 0, 0)), null, "Fortalece la espalda con tension constante.", 1, 5, null, 2, "Remo sentado en polea", "remo-polea", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6162), new TimeSpan(0, 0, 0, 0, 0)), null, "Trabaja cadena posterior y control lumbar.", 2, 3, null, 2, "Peso muerto rumano", "peso-muerto-rumano", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6163), new TimeSpan(0, 0, 0, 0, 0)), null, "Patron rey para fuerza y potencia del tren inferior.", 2, 3, null, 3, "Sentadilla libre", "sentadilla-libre", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000010"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6165), new TimeSpan(0, 0, 0, 0, 0)), null, "Aumenta volumen de cuadriceps con buena estabilidad.", 1, 4, null, 3, "Prensa de piernas", "prensa-piernas", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000011"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6167), new TimeSpan(0, 0, 0, 0, 0)), null, "Ejercicio unilateral para piernas y estabilidad.", 1, 2, null, 3, "Zancadas caminando", "zancadas-caminando", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000012"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6168), new TimeSpan(0, 0, 0, 0, 0)), null, "Enfocado en isquiotibiales y control de rodilla.", 1, 4, null, 3, "Curl femoral", "curl-femoral", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000013"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6170), new TimeSpan(0, 0, 0, 0, 0)), null, "Fortalece hombros y estabilidad del core.", 2, 3, null, 4, "Press militar", "press-militar", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6171), new TimeSpan(0, 0, 0, 0, 0)), null, "Aisla el deltoide medio para amplitud visual.", 1, 2, null, 4, "Elevaciones laterales", "elevaciones-laterales", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6172), new TimeSpan(0, 0, 0, 0, 0)), null, "Mejora la salud escapular y deltoides posterior.", 1, 5, null, 4, "Face pulls", "face-pulls", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000016"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6174), new TimeSpan(0, 0, 0, 0, 0)), null, "Combina empuje y rotacion para hombro completo.", 2, 2, null, 4, "Arnold press", "arnold-press", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000017"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6175), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimiento base para fuerza de biceps.", 1, 3, null, 5, "Curl con barra", "curl-barra", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000018"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6177), new TimeSpan(0, 0, 0, 0, 0)), null, "Trabaja braquial y antebrazo.", 1, 2, null, 5, "Curl martillo", "curl-martillo", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000019"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6178), new TimeSpan(0, 0, 0, 0, 0)), null, "Mayor estiramiento para hipertrofia de biceps.", 2, 2, null, 5, "Curl inclinado", "curl-inclinado", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6179), new TimeSpan(0, 0, 0, 0, 0)), null, "Tension continua durante todo el recorrido.", 1, 5, null, 5, "Curl en cable", "curl-cable", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000021"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6180), new TimeSpan(0, 0, 0, 0, 0)), null, "Aislamiento para la porcion larga del triceps.", 2, 3, null, 6, "Press frances", "press-frances", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000022"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6181), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimiento controlado para congestion del triceps.", 1, 5, null, 6, "Extensiones en polea", "extensiones-polea", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000023"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6182), new TimeSpan(0, 0, 0, 0, 0)), null, "Ejercicio accesible para iniciar trabajo de triceps.", 1, 6, null, 6, "Fondos en banco", "fondos-banco", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000024"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6183), new TimeSpan(0, 0, 0, 0, 0)), null, "Aislamiento ligero con enfoque tecnico.", 1, 2, null, 6, "Patada de triceps", "patada-triceps", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6184), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimiento basico para abdomen superior.", 1, 11, null, 7, "Crunch en colchoneta", "crunch-colchoneta", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000026"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6185), new TimeSpan(0, 0, 0, 0, 0)), null, "Mejora estabilidad del core.", 1, 1, null, 7, "Plancha frontal", "plancha-frontal", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6187), new TimeSpan(0, 0, 0, 0, 0)), null, "Trabaja abdomen inferior y control lumbo-pelvico.", 2, 11, null, 7, "Elevaciones de piernas", "elevaciones-piernas", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000028"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6190), new TimeSpan(0, 0, 0, 0, 0)), null, "Activa oblicuos y resistencia del core.", 1, 1, null, 7, "Russian twist", "russian-twist", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6192), new TimeSpan(0, 0, 0, 0, 0)), null, "Principal constructor de gluteo y potencia de cadera.", 2, 3, null, 8, "Hip thrust", "hip-thrust", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000030"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6193), new TimeSpan(0, 0, 0, 0, 0)), null, "Activacion inicial y tecnica de extension de cadera.", 1, 1, null, 8, "Puente de gluteos", "puente-gluteos", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000031"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6194), new TimeSpan(0, 0, 0, 0, 0)), null, "Aisla gluteo mayor con control.", 1, 5, null, 8, "Patada de gluteo en polea", "patada-gluteo-polea", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6195), new TimeSpan(0, 0, 0, 0, 0)), null, "Enfasis en gluteo medio y estabilidad.", 1, 4, null, 8, "Abduccion de cadera", "abduccion-cadera", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6196), new TimeSpan(0, 0, 0, 0, 0)), null, "Cardio de baja intensidad para gasto calorico sostenido.", 1, 9, null, 9, "Caminata en cinta", "caminata-cinta", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000034"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6198), new TimeSpan(0, 0, 0, 0, 0)), null, "Trabajo cardiovascular de impacto bajo.", 1, 8, null, 9, "Bicicleta estatica", "bicicleta-estatica", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6199), new TimeSpan(0, 0, 0, 0, 0)), null, "Cardio de cuerpo completo con enfoque tecnico.", 2, 10, null, 9, "Remo ergometro", "remo-ergometro", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000036"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6200), new TimeSpan(0, 0, 0, 0, 0)), null, "Condicionamiento intenso de cuerpo completo.", 3, 1, null, 9, "Burpees", "burpees", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000037"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6201), new TimeSpan(0, 0, 0, 0, 0)), null, "Ejercicio global de empuje y estabilidad.", 1, 1, null, 10, "Flexiones", "flexiones", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000038"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6202), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimiento funcional para tren inferior y core.", 1, 2, null, 10, "Sentadilla goblet", "sentadilla-goblet", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000039"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6203), new TimeSpan(0, 0, 0, 0, 0)), null, "Potencia, cardio y cadena posterior.", 2, 7, null, 10, "Kettlebell swing", "kettlebell-swing", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000040"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6205), new TimeSpan(0, 0, 0, 0, 0)), null, "Core dinamico con respuesta cardiovascular.", 1, 1, null, 10, "Mountain climbers", "mountain-climbers", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000041"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6206), new TimeSpan(0, 0, 0, 0, 0)), null, "Trabajo unilateral y coordinacion.", 1, 6, null, 3, "Step-up en banco", "step-up-banco", null, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000042"), new DateTimeOffset(new DateTime(2026, 5, 3, 17, 31, 35, 163, DateTimeKind.Unspecified).AddTicks(6207), new TimeSpan(0, 0, 0, 0, 0)), null, "Aislamiento dorsal y control escapular.", 2, 5, null, 2, "Pull over en polea", "pull-over-polea", null, null, null }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_exercise_catalog_MuscleGroup",
                table: "exercise_catalog",
                column: "MuscleGroup");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_catalog_Slug",
                table: "exercise_catalog",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_days_TrainingWeekId_DayOfWeek",
                table: "training_days",
                columns: new[] { "TrainingWeekId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_exercises_ExerciseId",
                table: "training_exercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_training_exercises_TrainingDayId_Order",
                table: "training_exercises",
                columns: new[] { "TrainingDayId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_phases_TrainingPlanId_Order",
                table: "training_phases",
                columns: new[] { "TrainingPlanId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_plans_CustomerId_Status",
                table: "training_plans",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_training_weeks_TrainingPhaseId_WeekNumber",
                table: "training_weeks",
                columns: new[] { "TrainingPhaseId", "WeekNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "training_exercises");

            migrationBuilder.DropTable(
                name: "exercise_catalog");

            migrationBuilder.DropTable(
                name: "training_days");

            migrationBuilder.DropTable(
                name: "training_weeks");

            migrationBuilder.DropTable(
                name: "training_phases");

            migrationBuilder.DropTable(
                name: "training_plans");

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 957, DateTimeKind.Unspecified).AddTicks(3811), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 957, DateTimeKind.Unspecified).AddTicks(3791), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 957, DateTimeKind.Unspecified).AddTicks(3807), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 957, DateTimeKind.Unspecified).AddTicks(3809), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "branches",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 957, DateTimeKind.Unspecified).AddTicks(3810), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 960, DateTimeKind.Unspecified).AddTicks(9206), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 960, DateTimeKind.Unspecified).AddTicks(9214), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 960, DateTimeKind.Unspecified).AddTicks(9216), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 960, DateTimeKind.Unspecified).AddTicks(9238), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 5, 3, 16, 22, 5, 960, DateTimeKind.Unspecified).AddTicks(9244), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
