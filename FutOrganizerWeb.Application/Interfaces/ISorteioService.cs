using FutOrganizerWeb.Application.DTOs;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface ISorteioService
    {
        Task<Guid> CriarSorteioAsync(SorteioRequest request);
        Task<Guid> ResortearAsync(Guid sorteioId, List<TimeRequest> novosTimes);
        Task<Guid?> CriarSorteioParaLobbyAsync(string codigoLobby, SorteioRequest request);
    }
}
