using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicPlayerMVVM.Migrations
{
    public partial class InitialCreat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Songs_Custom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_Custom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs_HipHop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_HipHop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs_Jazz",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_Jazz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs_Klassik",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_Klassik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs_Pop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_Pop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs_Rock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Artist = table.Column<string>(maxLength: 255, nullable: false),
                    Duration = table.Column<string>(maxLength: 10, nullable: true),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs_Rock", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Songs_Custom");

            migrationBuilder.DropTable(
                name: "Songs_HipHop");

            migrationBuilder.DropTable(
                name: "Songs_Jazz");

            migrationBuilder.DropTable(
                name: "Songs_Klassik");

            migrationBuilder.DropTable(
                name: "Songs_Pop");

            migrationBuilder.DropTable(
                name: "Songs_Rock");
        }
    }
}
