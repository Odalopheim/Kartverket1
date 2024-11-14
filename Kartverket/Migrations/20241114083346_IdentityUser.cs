using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kartverket.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BekreftPassord",
                table: "Brukere",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Brukernavn",
                table: "Brukere",
                type: "varchar(321)",
                maxLength: 321,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Epost",
                table: "Brukere",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Passord",
                table: "Brukere",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BekreftPassord",
                table: "Brukere");

            migrationBuilder.DropColumn(
                name: "Brukernavn",
                table: "Brukere");

            migrationBuilder.DropColumn(
                name: "Epost",
                table: "Brukere");

            migrationBuilder.DropColumn(
                name: "Passord",
                table: "Brukere");
        }
    }
}
