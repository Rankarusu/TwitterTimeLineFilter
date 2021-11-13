using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitterTimeLineFilterEF.Migrations
{
    public partial class Datetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TwitterUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "TwitterUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TwitterId",
                table: "TwitterUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TweetId = table.Column<long>(type: "INTEGER", nullable: false),
                    Html = table.Column<string>(type: "TEXT", nullable: true),
                    DateTime = table.Column<long>(type: "INTEGER", nullable: false),
                    TwitterUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tweets_TwitterUsers_TwitterUserId",
                        column: x => x.TwitterUserId,
                        principalTable: "TwitterUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_TwitterUserId",
                table: "Tweets",
                column: "TwitterUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tweets");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TwitterUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "TwitterUsers");

            migrationBuilder.DropColumn(
                name: "TwitterId",
                table: "TwitterUsers");
        }
    }
}
