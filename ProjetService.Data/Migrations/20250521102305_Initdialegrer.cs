using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initdialegrer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    heure_debut = table.Column<TimeSpan>(type: "time", nullable: false),
                    heure_fin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    tache_id = table.Column<int>(type: "int", nullable: false),
                    projet_id = table.Column<int>(type: "int", nullable: false),
                    liste_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planifications_Projets_projet_id",
                        column: x => x.projet_id,
                        principalTable: "Projets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Planifications_Taches_tache_id",
                        column: x => x.tache_id,
                        principalTable: "Taches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Planifications_projet_id",
                table: "Planifications",
                column: "projet_id");

            migrationBuilder.CreateIndex(
                name: "IX_Planifications_tache_id",
                table: "Planifications",
                column: "tache_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planifications");
        }
    }
}
