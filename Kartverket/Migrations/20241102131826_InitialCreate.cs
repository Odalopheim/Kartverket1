using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kartverket.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BrukerId",
                columns: table => new
                {
                    BrukerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Epost = table.Column<string>(type: "varchar(321)", maxLength: 321, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Brukernavn = table.Column<string>(type: "varchar(321)", maxLength: 321, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Passord = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrukerId", x => x.BrukerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GeoChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GeoJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoChange", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Geodata",
                columns: table => new
                {
                    GeoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Lat = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Lng = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geodata", x => x.GeoId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Gjester",
                columns: table => new
                {
                    GjestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gjester", x => x.GjestId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kategorier",
                columns: table => new
                {
                    KatNr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KatNavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorier", x => x.KatNr);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostSteder",
                columns: table => new
                {
                    PostNr = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sted = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostSteder", x => x.PostNr);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ansatte",
                columns: table => new
                {
                    AnsattId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FødselsDato = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BrukerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ansatte", x => x.AnsattId);
                    table.ForeignKey(
                        name: "FK_Ansatte_BrukerId_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "BrukerId",
                        principalColumn: "BrukerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vedlegg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FilNavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilData = table.Column<byte[]>(type: "longblob", nullable: false),
                    GeoChangeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vedlegg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vedlegg_GeoChange_GeoChangeId",
                        column: x => x.GeoChangeId,
                        principalTable: "GeoChange",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Brukere",
                columns: table => new
                {
                    BrukerNr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fornavn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Etternavn = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresse = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostNr = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brukere", x => x.BrukerNr);
                    table.ForeignKey(
                        name: "FK_Brukere_PostSteder_PostNr",
                        column: x => x.PostNr,
                        principalTable: "PostSteder",
                        principalColumn: "PostNr",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Innmeldere",
                columns: table => new
                {
                    InnmelderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BrukerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Innmeldere", x => x.InnmelderId);
                    table.ForeignKey(
                        name: "FK_Innmeldere_Brukere_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "Brukere",
                        principalColumn: "BrukerNr",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Innmeldinger",
                columns: table => new
                {
                    InnmeldingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Dato = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Beskrivelse = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BrukerId = table.Column<int>(type: "int", nullable: false),
                    KatNr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Innmeldinger", x => x.InnmeldingId);
                    table.ForeignKey(
                        name: "FK_Innmeldinger_Brukere_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "Brukere",
                        principalColumn: "BrukerNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Innmeldinger_Kategorier_KatNr",
                        column: x => x.KatNr,
                        principalTable: "Kategorier",
                        principalColumn: "KatNr",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Ansatte_BrukerId",
                table: "Ansatte",
                column: "BrukerId");

            migrationBuilder.CreateIndex(
                name: "IX_Brukere_PostNr",
                table: "Brukere",
                column: "PostNr");

            migrationBuilder.CreateIndex(
                name: "IX_Innmeldere_BrukerId",
                table: "Innmeldere",
                column: "BrukerId");

            migrationBuilder.CreateIndex(
                name: "IX_Innmeldinger_BrukerId",
                table: "Innmeldinger",
                column: "BrukerId");

            migrationBuilder.CreateIndex(
                name: "IX_Innmeldinger_KatNr",
                table: "Innmeldinger",
                column: "KatNr");

            migrationBuilder.CreateIndex(
                name: "IX_Vedlegg_GeoChangeId",
                table: "Vedlegg",
                column: "GeoChangeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ansatte");

            migrationBuilder.DropTable(
                name: "Geodata");

            migrationBuilder.DropTable(
                name: "Gjester");

            migrationBuilder.DropTable(
                name: "Innmeldere");

            migrationBuilder.DropTable(
                name: "Innmeldinger");

            migrationBuilder.DropTable(
                name: "Vedlegg");

            migrationBuilder.DropTable(
                name: "BrukerId");

            migrationBuilder.DropTable(
                name: "Brukere");

            migrationBuilder.DropTable(
                name: "Kategorier");

            migrationBuilder.DropTable(
                name: "GeoChange");

            migrationBuilder.DropTable(
                name: "PostSteder");
        }
    }
}
