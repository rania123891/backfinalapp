using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initdialgedfdgrer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "liste_id",
                table: "Planifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Planifications_Date",
                table: "Planifications",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Planifications_Date_HeureDebut",
                table: "Planifications",
                columns: new[] { "Date", "heure_debut" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Planifications_Date",
                table: "Planifications");

            migrationBuilder.DropIndex(
                name: "IX_Planifications_Date_HeureDebut",
                table: "Planifications");

            migrationBuilder.AlterColumn<int>(
                name: "liste_id",
                table: "Planifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
