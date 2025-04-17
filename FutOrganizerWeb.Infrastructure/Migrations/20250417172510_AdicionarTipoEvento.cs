using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTipoEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Eventos",
                newName: "ValorInscricao");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Eventos",
                newName: "Observacoes");

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "ValorInscricao",
                table: "Eventos",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "Observacoes",
                table: "Eventos",
                newName: "Descricao");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Eventos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Eventos",
                type: "datetime2",
                nullable: true);
        }
    }
}
