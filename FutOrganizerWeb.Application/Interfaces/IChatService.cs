using FutOrganizerWeb.Application.DTOs;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IChatService
    {
        Task SalvarMensagemAsync(string codigoSala, string nomeUsuario, string mensagem);
        Task<List<MensagemChatDTO>> ObterMensagensAsync(string codigoSala);
    }
}
