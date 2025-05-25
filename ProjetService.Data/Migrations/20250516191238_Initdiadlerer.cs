using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initdiadlerer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taches_Projets_ProjetId",
                table: "Taches");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "Taches");

            migrationBuilder.DropColumn(
                name: "DateEcheance",
                table: "Taches");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Taches");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Taches");

            migrationBuilder.AlterColumn<int>(
                name: "ProjetId",
                table: "Taches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Taches_Projets_ProjetId",
                table: "Taches",
                column: "ProjetId",
                principalTable: "Projets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taches_Projets_ProjetId",
                table: "Taches");

            migrationBuilder.AlterColumn<int>(
                name: "ProjetId",
                table: "Taches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "Taches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEcheance",
                table: "Taches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Taches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Taches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Taches_Projets_ProjetId",
                table: "Taches",
                column: "ProjetId",
                principalTable: "Projets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
