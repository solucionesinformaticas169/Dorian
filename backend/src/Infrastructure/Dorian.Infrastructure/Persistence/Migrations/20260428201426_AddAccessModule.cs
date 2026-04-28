using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActiveMembershipEndsAtUtc",
                table: "customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActiveMembershipStartsAtUtc",
                table: "customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "access_passes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QrCodeValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_passes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_access_passes_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "check_ins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CheckedInByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_ins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_ins_branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_check_ins_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_check_ins_users_CheckedInByUserId",
                        column: x => x.CheckedInByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_access_passes_CustomerId",
                table: "access_passes",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_access_passes_QrCodeValue",
                table: "access_passes",
                column: "QrCodeValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_ins_BranchId_CheckedInAt",
                table: "check_ins",
                columns: new[] { "BranchId", "CheckedInAt" });

            migrationBuilder.CreateIndex(
                name: "IX_check_ins_CheckedInByUserId",
                table: "check_ins",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_check_ins_CustomerId_CheckedInAt",
                table: "check_ins",
                columns: new[] { "CustomerId", "CheckedInAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_passes");

            migrationBuilder.DropTable(
                name: "check_ins");

            migrationBuilder.DropColumn(
                name: "ActiveMembershipEndsAtUtc",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "ActiveMembershipStartsAtUtc",
                table: "customers");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 19, 54, 28, 930, DateTimeKind.Unspecified).AddTicks(1278), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 19, 54, 28, 930, DateTimeKind.Unspecified).AddTicks(1281), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 19, 54, 28, 930, DateTimeKind.Unspecified).AddTicks(1282), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 19, 54, 28, 930, DateTimeKind.Unspecified).AddTicks(1283), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAtUtc",
                value: new DateTimeOffset(new DateTime(2026, 4, 28, 19, 54, 28, 930, DateTimeKind.Unspecified).AddTicks(1284), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
