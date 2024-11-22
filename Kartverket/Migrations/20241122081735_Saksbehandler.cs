using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kartverket.Migrations
{
    public partial class Saksbehandler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Saksbehandler",
                table: "GeoChanges",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Oppdaterer Saksbehandler til å være nullable
            // håndterer konverteringen fra en tom string til NULL i databasen
            migrationBuilder.Sql(
                @"
                UPDATE GeoChanges
                SET Saksbehandler = NULL
                WHERE Saksbehandler = ''
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GeoChanges",
                keyColumn: "Saksbehandler",
                keyValue: null,
                column: "Saksbehandler",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Saksbehandler",
                table: "GeoChanges",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Tilbakestiller Saksbehandler til å være nullable
            // Håndterer konvertering fra tom string til NULL i databasen
            migrationBuilder.Sql(
            @"
            UPDATE GeoChanges
            SET Saksbehandler = ''
            WHERE Saksbehandler IS NULL
            ");
        }
    }
}