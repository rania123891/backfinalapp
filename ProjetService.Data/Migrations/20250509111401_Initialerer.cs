using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initialerer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listes_Tableaux_TableauId",
                table: "Listes");

            migrationBuilder.DropTable(
                name: "Tableaux");

            migrationBuilder.DropIndex(
                name: "IX_Listes_TableauId",
                table: "Listes");

            migrationBuilder.DropColumn(
                name: "TableauId",
                table: "Listes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TableauId",
                table: "Listes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tableaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjetId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tableaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tableaux_Projets_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listes_TableauId",
                table: "Listes",
                column: "TableauId");

            migrationBuilder.CreateIndex(
                name: "IX_Tableaux_ProjetId",
                table: "Tableaux",
                column: "ProjetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listes_Tableaux_TableauId",
                table: "Listes",
                column: "TableauId",
                principalTable: "Tableaux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
