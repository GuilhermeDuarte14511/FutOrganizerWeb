using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAtualizacaoToSorteio2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoLobby",
                table: "Partidas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JogadoresLobby",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogadoresLobby", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JogadoresLobby_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JogadoresLobby_PartidaId",
                table: "JogadoresLobby",
                column: "PartidaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JogadoresLobby");

            migrationBuilder.DropColumn(
                name: "CodigoLobby",
                table: "Partidas");
        }
    }
}
