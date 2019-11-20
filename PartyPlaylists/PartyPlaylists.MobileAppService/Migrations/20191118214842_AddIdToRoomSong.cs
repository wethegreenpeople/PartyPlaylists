using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddIdToRoomSong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomSong",
                table: "RoomSong");

            migrationBuilder.RenameTable(
                name: "RoomSong",
                newName: "RoomSongs");

            migrationBuilder.RenameIndex(
                name: "IX_RoomSong_SongId",
                table: "RoomSongs",
                newName: "IX_RoomSongs_SongId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoomSongs",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomSongs",
                table: "RoomSongs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomSongs_RoomId",
                table: "RoomSongs",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSongs_Rooms_RoomId",
                table: "RoomSongs",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSongs_Songs_SongId",
                table: "RoomSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomSongs_Rooms_RoomId",
                table: "RoomSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomSongs_Songs_SongId",
                table: "RoomSongs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomSongs",
                table: "RoomSongs");

            migrationBuilder.DropIndex(
                name: "IX_RoomSongs_RoomId",
                table: "RoomSongs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoomSongs");

            migrationBuilder.RenameTable(
                name: "RoomSongs",
                newName: "RoomSong");

            migrationBuilder.RenameIndex(
                name: "IX_RoomSongs_SongId",
                table: "RoomSong",
                newName: "IX_RoomSong_SongId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomSong",
                table: "RoomSong",
                columns: new[] { "RoomId", "SongId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSong_Rooms_RoomId",
                table: "RoomSong",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomSong_Songs_SongId",
                table: "RoomSong",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
