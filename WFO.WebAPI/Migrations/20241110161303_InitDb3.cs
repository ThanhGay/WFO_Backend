using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFO.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitDb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OTP",
                schema: "auth",
                table: "AuthUser",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTP",
                schema: "auth",
                table: "AuthUser");
        }
    }
}
