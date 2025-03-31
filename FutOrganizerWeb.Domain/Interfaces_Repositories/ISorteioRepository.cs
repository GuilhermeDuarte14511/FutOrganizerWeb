using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface ISorteioRepository
    {
        Task<Guid> CriarPartidaComSorteioAsync(Partida partida);
        Task<Sorteio?> ObterSorteioComTimesAsync(Guid sorteioId);
        Task AtualizarSorteioAsync(Sorteio sorteio);
        Task SalvarAsync();
        Task RemoverPartidaCompletaAsync(Guid partidaId);

    }
}
