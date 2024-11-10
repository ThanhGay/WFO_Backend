using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFO.WebAPI.Migrations.ProductDb
{
    /// <inheritdoc />
    public partial class InitDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                schema: "prod",
                table: "ProdProduct",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                schema: "prod",
                table: "ProdProduct");
        }
    }
}
