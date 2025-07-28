using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTextDateColumnForComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TextDate",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextDate",
                table: "Comments");
        }
    }
}
