using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IPartidaRepository
    {
        Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina);
        Task<Partida?> ObterPartidaComSorteioAsync(Guid partidaId);
        Task<Partida?> ObterPorCodigoAsync(string codigo);
        Task SalvarAsync();
        Task CriarPartidaAsync(Partida partida);
        Task AdicionarJogadorAsync(JogadorLobby jogador);
        Task RemoverJogadorAsync(string codigo, Guid jogadorId);
        Task<List<Partida>> ObterPartidasPaginadasPorUsuarioAsync(Guid usuarioId, int page, int pageSize);

    }
}
