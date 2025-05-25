using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contenu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpediteurId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinataireId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnvoyeLe = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
