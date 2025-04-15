using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FutOrganizerWeb.Hubs
{
    public class LobbyChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IPartidaService _partidaService;

        // ConnectionId => (codigoSala, identificador único)
        public static readonly ConcurrentDictionary<string, (string Sala, string Identificador)> UsuariosConectados = new();

        public LobbyChatHub(IChatService chatService, IPartidaService partidaService)
        {
            _chatService = chatService;
            _partidaService = partidaService;
        }

        public async Task EnviarMensagem(string codigoSala, string identificador, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(codigoSala) || string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(mensagem))
                return;

            // Tenta obter o nome do jogador pela partida
            var partida = await _partidaService.ObterPorCodigoAsync(codigoSala);
            var nomeJogador = partida?.JogadoresLobby
                .FirstOrDefault(j => LobbyHelper.ObterIdentificadorJogador(j.UsuarioAutenticadoId, j.Nome) == identificador)?.Nome ?? "Desconhecido";

            await _partidaService.AtualizarUltimaAtividadeAsync(codigoSala, nomeJogador);

            var mensagemSalva = await _chatService.SalvarMensagemAsync(codigoSala, nomeJogador, mensagem);
            if (mensagemSalva == null) return;

            var horaEnvio = mensagemSalva.DataEnvio.ToString("HH:mm");

            await Clients.Group(codigoSala).SendAsync("ReceberMensagem", nomeJogador, mensagem, horaEnvio);
        }

        public async Task UsuarioDigitando(string codigoSala, string identificador)
        {
            if (!string.IsNullOrWhiteSpace(codigoSala) && !string.IsNullOrWhiteSpace(identificador))
            {
                var partida = await _partidaService.ObterPorCodigoAsync(codigoSala);
                var nomeJogador = partida?.JogadoresLobby
                    .FirstOrDefault(j => LobbyHelper.ObterIdentificadorJogador(j.UsuarioAutenticadoId, j.Nome) == identificador)?.Nome ?? "Jogador";

                await _partidaService.AtualizarUltimaAtividadeAsync(codigoSala, nomeJogador);
                await Clients.Group(codigoSala).SendAsync("MostrarDigitando", nomeJogador);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var codigoSala = httpContext?.Request.Query["codigoSala"].ToString();
            var nomeUsuario = httpContext?.Request.Query["nome"].ToString();
            var usuarioIdStr = httpContext?.Request.Query["jogadorId"].ToString();

            Guid? usuarioId = Guid.TryParse(usuarioIdStr, out var guid) ? guid : null;
            var identificador = LobbyHelper.ObterIdentificadorJogador(usuarioId, nomeUsuario);

            if (!string.IsNullOrWhiteSpace(codigoSala) && !string.IsNullOrWhiteSpace(identificador))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, codigoSala);
                UsuariosConectados.TryAdd(Context.ConnectionId, (codigoSala, identificador));

                await _partidaService.AtualizarUltimaAtividadeAsync(codigoSala, nomeUsuario ?? "Jogador");
                await NotificarUsuariosOnline(codigoSala);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UsuariosConectados.TryRemove(Context.ConnectionId, out var info))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.Sala);
                await NotificarUsuariosOnline(info.Sala);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task NotificarUsuariosOnline(string codigoSala)
        {
            var partida = await _partidaService.ObterPorCodigoAsync(codigoSala);
            if (partida == null) return;

            var conexoes = UsuariosConectados
                .Where(x => x.Value.Sala == codigoSala)
                .Select(x => x.Value.Identificador)
                .ToHashSet();

            var jogadoresOnline = partida.JogadoresLobby
                .Where(j =>
                    conexoes.Contains(
                        LobbyHelper.ObterIdentificadorJogador(j.UsuarioAutenticadoId, j.Nome)
                    ))
                .Select(j => j.Nome)
                .Distinct()
                .ToList();

            await Clients.Group(codigoSala).SendAsync("AtualizarUsuariosOnline", jogadoresOnline);
        }
    }
}
