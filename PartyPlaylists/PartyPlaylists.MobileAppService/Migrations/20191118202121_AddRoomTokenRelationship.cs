using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddRoomTokenRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongToken");

            migrationBuilder.CreateTable(
                name: "RoomSongToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(nullable: false),
                    RoomSongRoomId = table.Column<int>(nullable: true),
                    RoomSongSongId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomSongToken", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_RoomSongToken_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomSongToken_RoomSong_RoomSongRoomId_RoomSongSongId",
                        columns: x => new { x.RoomSongRoomId, x.RoomSongSongId },
                        principalTable: "RoomSong",
                        principalColumns: new[] { "RoomId", "SongId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomSongToken_RoomSongRoomId_RoomSongSongId",
                table: "RoomSongToken",
                columns: new[] { "RoomSongRoomId", "RoomSongSongId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomSongToken");

            migrationBuilder.CreateTable(
                name: "SongToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(nullable: false),
                    SongId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongToken", x => new { x.TokenId, x.SongId });
                    table.ForeignKey(
                        name: "FK_SongToken_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongToken_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongToken_SongId",
                table: "SongToken",
                column: "SongId");
        }
    }
}
