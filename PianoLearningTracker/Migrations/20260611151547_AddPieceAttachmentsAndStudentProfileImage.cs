using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PianoLearningTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddPieceAttachmentsAndStudentProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PieceAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PieceId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieceAttachments_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImagePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImagePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImagePath",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_PieceAttachments_PieceId",
                table: "PieceAttachments",
                column: "PieceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PieceAttachments");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "Students");
        }
    }
}
