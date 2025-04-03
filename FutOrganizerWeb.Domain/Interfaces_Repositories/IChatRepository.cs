using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Application.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task AdicionarMensagemAsync(MensagemChat mensagem);
        Task<List<MensagemChat>> ObterMensagensPorCodigoSalaAsync(string codigoSala);
    }
}
