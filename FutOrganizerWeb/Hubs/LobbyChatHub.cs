using FutOrganizerWeb.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Hubs
{
    public class LobbyChatHub : Hub
    {
        private readonly IChatService _chatService;

        public LobbyChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task EnviarMensagem(string codigoSala, string usuario, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(codigoSala) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(mensagem))
                return;

            // Salva no banco
            await _chatService.SalvarMensagemAsync(codigoSala, usuario, mensagem);

            // Envia para todos do grupo, incluindo horário
            var horaEnvio = DateTime.Now.ToString("HH:mm");
            await Clients.Group(codigoSala).SendAsync("ReceberMensagem", usuario, mensagem, horaEnvio);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var codigoSala = httpContext?.Request.Query["codigoSala"].ToString();

            if (!string.IsNullOrWhiteSpace(codigoSala))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, codigoSala);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var codigoSala = httpContext?.Request.Query["codigoSala"].ToString();

            if (!string.IsNullOrWhiteSpace(codigoSala))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, codigoSala);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
