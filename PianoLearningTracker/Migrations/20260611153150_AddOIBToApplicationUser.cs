using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PianoLearningTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddOIBToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OIB",
                table: "AspNetUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OIB",
                table: "AspNetUsers");
        }
    }
}
