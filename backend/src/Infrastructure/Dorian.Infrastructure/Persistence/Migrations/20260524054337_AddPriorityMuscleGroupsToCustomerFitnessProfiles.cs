using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dorian.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityMuscleGroupsToCustomerFitnessProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriorityMuscleGroupsSerialized",
                table: "customer_fitness_profiles",
                type: "character varying(240)",
                maxLength: 240,
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriorityMuscleGroupsSerialized",
                table: "customer_fitness_profiles");

        }
    }
}
