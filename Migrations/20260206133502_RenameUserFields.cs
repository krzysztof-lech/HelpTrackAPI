using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nazwisko",
                table: "user",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Imie",
                table: "user",
                newName: "FirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user",
                newName: "Nazwisko");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user",
                newName: "Imie");
        }
    }
}
