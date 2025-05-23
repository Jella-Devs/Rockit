using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rockit.Migrations
{
    /// <inheritdoc />
    public partial class SecondVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Route",
                table: "Songs",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "Players",
                table: "Artists",
                newName: "Rp");

            migrationBuilder.RenameColumn(
                name: "Front",
                table: "Artists",
                newName: "Picture");

            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "Songs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rp",
                table: "Songs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Rp",
                table: "Songs");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Songs",
                newName: "Route");

            migrationBuilder.RenameColumn(
                name: "Rp",
                table: "Artists",
                newName: "Players");

            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Artists",
                newName: "Front");
        }
    }
}
