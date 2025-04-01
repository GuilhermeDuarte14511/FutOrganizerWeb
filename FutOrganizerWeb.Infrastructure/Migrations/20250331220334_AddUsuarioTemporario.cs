using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioTemporario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioTemporarioId",
                table: "Partidas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsuariosTemporarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosTemporarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_UsuarioTemporarioId",
                table: "Partidas",
                column: "UsuarioTemporarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_UsuariosTemporarios_UsuarioTemporarioId",
                table: "Partidas",
                column: "UsuarioTemporarioId",
                principalTable: "UsuariosTemporarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidas_UsuariosTemporarios_UsuarioTemporarioId",
                table: "Partidas");

            migrationBuilder.DropTable(
                name: "UsuariosTemporarios");

            migrationBuilder.DropIndex(
                name: "IX_Partidas_UsuarioTemporarioId",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "UsuarioTemporarioId",
                table: "Partidas");
        }
    }
}
