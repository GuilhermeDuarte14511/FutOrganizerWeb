using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioComRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCriadorId",
                table: "Partidas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCriadorId",
                table: "Eventos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_UsuarioCriadorId",
                table: "Partidas",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuarioCriadorId",
                table: "Eventos",
                column: "UsuarioCriadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioCriadorId",
                table: "Eventos",
                column: "UsuarioCriadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_Usuarios_UsuarioCriadorId",
                table: "Partidas",
                column: "UsuarioCriadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioCriadorId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidas_Usuarios_UsuarioCriadorId",
                table: "Partidas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Partidas_UsuarioCriadorId",
                table: "Partidas");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_UsuarioCriadorId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UsuarioCriadorId",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "UsuarioCriadorId",
                table: "Eventos");
        }
    }
}
