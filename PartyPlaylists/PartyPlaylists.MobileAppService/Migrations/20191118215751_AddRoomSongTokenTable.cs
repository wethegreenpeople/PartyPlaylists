using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddRoomSongTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomSongTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TokenId = table.Column<int>(nullable: false),
                    RoomSongId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomSongTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomSongTokens_RoomSongs_RoomSongId",
                        column: x => x.RoomSongId,
                        principalTable: "RoomSongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomSongTokens_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomSongTokens_RoomSongId",
                table: "RoomSongTokens",
                column: "RoomSongId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomSongTokens_TokenId",
                table: "RoomSongTokens",
                column: "TokenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomSongTokens");
        }
    }
}
