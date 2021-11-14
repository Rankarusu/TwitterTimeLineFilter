using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitterTimeLineFilterEF.Migrations
{
    public partial class checkforretweets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRetweet",
                table: "Tweets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRetweet",
                table: "Tweets");
        }
    }
}
