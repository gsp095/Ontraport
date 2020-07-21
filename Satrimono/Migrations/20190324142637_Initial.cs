using HanumanInstitute.CommonWeb;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HanumanInstitute.Satrimono.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CheckNotNull(nameof(migrationBuilder));

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Key = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: false, defaultValue: ""),
                    Title = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: true),
                    Subtitle = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: true),
                    Author = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: true),
                    Accuracy = table.Column<double>(nullable: true),
                    Vibration = table.Column<int>(nullable: true),
                    IsFiction = table.Column<bool>(nullable: false, defaultValue: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "Index_Author",
                table: "Book",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "Index_BookId",
                table: "Book",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_BookKey",
                table: "Book",
                column: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CheckNotNull(nameof(migrationBuilder));

            migrationBuilder.DropTable(
                name: "Book");
        }
    }
}
