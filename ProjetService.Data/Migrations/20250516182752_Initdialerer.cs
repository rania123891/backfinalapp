using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initdialerer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taches_Listes_ListeId",
                table: "Taches");

            migrationBuilder.DropTable(
                name: "Listes");

            migrationBuilder.DropIndex(
                name: "IX_Taches_ListeId",
                table: "Taches");

            migrationBuilder.DropColumn(
                name: "ListeId",
                table: "Taches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListeId",
                table: "Taches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Listes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Taches_ListeId",
                table: "Taches",
                column: "ListeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taches_Listes_ListeId",
                table: "Taches",
                column: "ListeId",
                principalTable: "Listes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
