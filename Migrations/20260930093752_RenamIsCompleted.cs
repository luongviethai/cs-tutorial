using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS_Tutorial.Migrations
{
    /// <inheritdoc />
    public partial class RenamIsCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "Todos");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Todos",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Todos");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "Todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
