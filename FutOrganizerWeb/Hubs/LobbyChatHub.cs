using FutOrganizerWeb.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Hubs
{
    public class LobbyChatHub : Hub
    {
        private readonly IChatService _chatService;

        // ConnectionId => (codigoSala, nomeUsuario)
        public static readonly ConcurrentDictionary<string, (string Sala, string Nome)> UsuariosConectados = new();

        public LobbyChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task EnviarMensagem(string codigoSala, string usuario, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(codigoSala) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(mensagem))
                return;

            await _chatService.SalvarMensagemAsync(codigoSala, usuario, mensagem);

            var agora = DateTime.UtcNow;
            await Clients.Group(codigoSala).SendAsync("ReceberMensagem", usuario, mensagem, agora);
        }

        public async Task UsuarioDigitando(string codigoSala, string usuario)
        {
            if (!string.IsNullOrWhiteSpace(codigoSala) && !string.IsNullOrWhiteSpace(usuario))
            {
                await Clients.Group(codigoSala).SendAsync("MostrarDigitando", usuario);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var codigoSala = httpContext?.Request.Query["codigoSala"].ToString();
            var nomeUsuario = httpContext?.Request.Query["nome"].ToString();

            if (!string.IsNullOrWhiteSpace(codigoSala) && !string.IsNullOrWhiteSpace(nomeUsuario))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, codigoSala);
                UsuariosConectados.TryAdd(Context.ConnectionId, (codigoSala, nomeUsuario));

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
            var usuariosOnline = UsuariosConectados
                .Where(kvp => kvp.Value.Sala == codigoSala)
                .Select(kvp => kvp.Value.Nome)
                .Distinct()
                .ToList();

            await Clients.Group(codigoSala).SendAsync("AtualizarUsuariosOnline", usuariosOnline);
        }
    }
}
