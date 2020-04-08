using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddRatedByColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
