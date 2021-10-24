using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitterTimeLineFilterEF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwitterUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitterUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwitterUserUserTag",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitterUserUserTag", x => new { x.TagsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TwitterUserUserTag_TwitterUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "TwitterUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TwitterUserUserTag_UserTags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "UserTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwitterUserUserTag_UsersId",
                table: "TwitterUserUserTag",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitterUserUserTag");

            migrationBuilder.DropTable(
                name: "TwitterUsers");

            migrationBuilder.DropTable(
                name: "UserTags");
        }
    }
}
