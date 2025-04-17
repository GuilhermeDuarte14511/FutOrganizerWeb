using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesPelada",
                columns: table => new
                {
                    JogadoresPorTime = table.Column<int>(type: "int", nullable: false),
                    TemGoleiroFixo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendGridTemplateId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Goleiros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goleiros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenRecuperacaoSenha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCriadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Usuarios_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estatisticas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeJogador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioAutenticadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JogadorLobbyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gols = table.Column<int>(type: "int", nullable: false),
                    Assistencias = table.Column<int>(type: "int", nullable: false),
                    Defesas = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estatisticas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estatisticas_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsuarioCriadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodigoLobby = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioTemporarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidas_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Partidas_UsuariosTemporarios_UsuarioTemporarioId",
                        column: x => x.UsuarioTemporarioId,
                        principalTable: "UsuariosTemporarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Partidas_Usuarios_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JogadoresLobby",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioAutenticadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtividade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_JogadoresLobby_Usuarios_UsuarioAutenticadoId",
                        column: x => x.UsuarioAutenticadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JogadoresLobby_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MensagensChat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeJogador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHoraEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensChat_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sorteios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sorteios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sorteios_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoleiroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SorteioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Times_Goleiros_GoleiroId",
                        column: x => x.GoleiroId,
                        principalTable: "Goleiros",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Times_Sorteios_SorteioId",
                        column: x => x.SorteioId,
                        principalTable: "Sorteios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Confrontos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SorteioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GolsTimeA = table.Column<int>(type: "int", nullable: false),
                    GolsTimeB = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confrontos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Confrontos_Sorteios_SorteioId",
                        column: x => x.SorteioId,
                        principalTable: "Sorteios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Confrontos_Times_TimeAId",
                        column: x => x.TimeAId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Confrontos_Times_TimeBId",
                        column: x => x.TimeBId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jogadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioAutenticadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JogadorLobbyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UltimaAtividade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogadores_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assistencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfrontoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Minuto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assistencias_Confrontos_ConfrontoId",
                        column: x => x.ConfrontoId,
                        principalTable: "Confrontos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assistencias_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfrontoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Minuto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gols_Confrontos_ConfrontoId",
                        column: x => x.ConfrontoId,
                        principalTable: "Confrontos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gols_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assistencias_ConfrontoId",
                table: "Assistencias",
                column: "ConfrontoId");

            migrationBuilder.CreateIndex(
                name: "IX_Assistencias_JogadorId",
                table: "Assistencias",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Confrontos_SorteioId",
                table: "Confrontos",
                column: "SorteioId");

            migrationBuilder.CreateIndex(
                name: "IX_Confrontos_TimeAId",
                table: "Confrontos",
                column: "TimeAId");

            migrationBuilder.CreateIndex(
                name: "IX_Confrontos_TimeBId",
                table: "Confrontos",
                column: "TimeBId");

            migrationBuilder.CreateIndex(
                name: "IX_Estatisticas_EventoId",
                table: "Estatisticas",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuarioCriadorId",
                table: "Eventos",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gols_ConfrontoId",
                table: "Gols",
                column: "ConfrontoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gols_JogadorId",
                table: "Gols",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_TimeId",
                table: "Jogadores",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_JogadoresLobby_PartidaId",
                table: "JogadoresLobby",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_JogadoresLobby_UsuarioAutenticadoId",
                table: "JogadoresLobby",
                column: "UsuarioAutenticadoId");

            migrationBuilder.CreateIndex(
                name: "IX_JogadoresLobby_UsuarioId",
                table: "JogadoresLobby",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChat_PartidaId",
                table: "MensagensChat",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_EventoId",
                table: "Partidas",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_UsuarioCriadorId",
                table: "Partidas",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_UsuarioTemporarioId",
                table: "Partidas",
                column: "UsuarioTemporarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorteios_PartidaId",
                table: "Sorteios",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Times_GoleiroId",
                table: "Times",
                column: "GoleiroId");

            migrationBuilder.CreateIndex(
                name: "IX_Times_SorteioId",
                table: "Times",
                column: "SorteioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assistencias");

            migrationBuilder.DropTable(
                name: "ConfiguracoesPelada");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "Estatisticas");

            migrationBuilder.DropTable(
                name: "Gols");

            migrationBuilder.DropTable(
                name: "JogadoresLobby");

            migrationBuilder.DropTable(
                name: "MensagensChat");

            migrationBuilder.DropTable(
                name: "Confrontos");

            migrationBuilder.DropTable(
                name: "Jogadores");

            migrationBuilder.DropTable(
                name: "Times");

            migrationBuilder.DropTable(
                name: "Goleiros");

            migrationBuilder.DropTable(
                name: "Sorteios");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "UsuariosTemporarios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
