namespace Scheduler.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Friads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserFreands",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    FriandId = table.Column<string>(nullable: false),
                    FriendId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserFreands", x => new { x.ApplicationUserId, x.FriandId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserFreands_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationUserFreands_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserFreands_FriendId",
                table: "ApplicationUserFreands",
                column: "FriendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserFreands");
        }
    }
}
