using FutOrganizerWeb.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IConfrontoService
    {
        Task CriarConfrontoAsync(Guid sorteioId, Guid timeAId, Guid timeBId);
        Task<List<ConfrontoDTO>> ObterConfrontosPorSorteioAsync(Guid sorteioId);
        Task AdicionarGolAsync(Guid confrontoId, Guid jogadorId);
        Task AdicionarAssistenciaAsync(Guid confrontoId, Guid jogadorId);
        Task AtualizarPlacarAsync(Guid confrontoId, int golsA, int golsB);

    }
}
