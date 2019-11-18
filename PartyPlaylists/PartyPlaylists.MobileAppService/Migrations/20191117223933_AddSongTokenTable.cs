using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddSongTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_RoomSong_RoomSongRoomId_RoomSongSongId",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_RoomSongRoomId_RoomSongSongId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "RoomSongRoomId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "RoomSongSongId",
                table: "Tokens");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongToken");

            migrationBuilder.AddColumn<int>(
                name: "RoomSongRoomId",
                table: "Tokens",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomSongSongId",
                table: "Tokens",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_RoomSongRoomId_RoomSongSongId",
                table: "Tokens",
                columns: new[] { "RoomSongRoomId", "RoomSongSongId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_RoomSong_RoomSongRoomId_RoomSongSongId",
                table: "Tokens",
                columns: new[] { "RoomSongRoomId", "RoomSongSongId" },
                principalTable: "RoomSong",
                principalColumns: new[] { "RoomId", "SongId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
