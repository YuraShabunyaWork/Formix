﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formix.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlPhoto",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlPhoto",
                table: "Reviews");
        }
    }
}
