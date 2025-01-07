using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Way2SendApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovedColumnToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "Tasks");
        }
    }
}
