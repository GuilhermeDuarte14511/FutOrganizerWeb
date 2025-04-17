using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IEventoRepository
    {
        Task<Evento?> ObterPorIdAsync(Guid id);
        Task<List<Evento>> ObterTodosDoUsuarioAsync(Guid usuarioId);
        Task CriarAsync(Evento evento);
        Task AtualizarAsync(Evento evento);
        Task ExcluirAsync(Guid id);
    }
}
