using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IPartidaRepository
    {
        Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId);
        Task<Partida?> ObterPartidaComSorteioAsync(Guid partidaId);
    }
}
