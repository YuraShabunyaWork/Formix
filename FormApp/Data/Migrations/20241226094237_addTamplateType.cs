using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formix.Migrations
{
    /// <inheritdoc />
    public partial class addTamplateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TamplateType",
                table: "Tamplates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TamplateType",
                table: "Tamplates");
        }
    }
}
