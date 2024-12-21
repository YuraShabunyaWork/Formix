using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formix.Migrations
{
    /// <inheritdoc />
    public partial class addPhotoLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountLike",
                table: "Tamplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UrlPhoto",
                table: "Tamplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountLike",
                table: "Tamplates");

            migrationBuilder.DropColumn(
                name: "UrlPhoto",
                table: "Tamplates");
        }
    }
}
