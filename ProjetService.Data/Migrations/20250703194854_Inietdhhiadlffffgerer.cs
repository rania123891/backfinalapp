using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inietdhhiadlffffgerer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaisiesTemps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    ProjetId = table.Column<int>(type: "int", nullable: false),
                    TacheId = table.Column<int>(type: "int", nullable: true),
                    DateTravail = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureDebut = table.Column<TimeSpan>(type: "time", nullable: false),
                    HeureFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    DureeHeures = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaisiesTemps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaisiesTemps_Projets_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaisiesTemps_Taches_TacheId",
                        column: x => x.TacheId,
                        principalTable: "Taches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaisiesTemps_ProjetId_DateTravail",
                table: "SaisiesTemps",
                columns: new[] { "ProjetId", "DateTravail" });

            migrationBuilder.CreateIndex(
                name: "IX_SaisiesTemps_TacheId",
                table: "SaisiesTemps",
                column: "TacheId");

            migrationBuilder.CreateIndex(
                name: "IX_SaisiesTemps_UtilisateurId_DateTravail",
                table: "SaisiesTemps",
                columns: new[] { "UtilisateurId", "DateTravail" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaisiesTemps");
        }
    }
}
