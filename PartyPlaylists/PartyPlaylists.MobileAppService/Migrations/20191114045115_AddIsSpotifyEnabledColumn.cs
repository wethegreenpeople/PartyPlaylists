using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class AddIsSpotifyEnabledColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpotifyEnabled",
                table: "Rooms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpotifyEnabled",
                table: "Rooms");
        }
    }
}
