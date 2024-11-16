using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFO.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitDb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                schema: "auth",
                table: "AuthUser",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                schema: "auth",
                table: "AuthUser");
        }
    }
}
