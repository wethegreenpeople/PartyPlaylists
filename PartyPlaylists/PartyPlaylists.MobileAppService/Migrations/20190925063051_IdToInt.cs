using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyPlaylists.MobileAppService.Migrations
{
    public partial class IdToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Rooms",
                nullable: false,
                oldClrType: typeof(byte[]))
                .Annotation("MySQL:AutoIncrement", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "Rooms",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("MySQL:AutoIncrement", true);
        }
    }
}
