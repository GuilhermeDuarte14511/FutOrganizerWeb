using FutOrganizerWeb.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IEventoService
    {
        Task<EventoDTO?> ObterPorIdAsync(Guid id);
        Task<List<EventoDTO>> ListarEventosDoUsuarioAsync(Guid usuarioId);
        Task<Guid> CriarEventoAsync(EventoDTO dto);
        Task<bool> EditarEventoAsync(EventoDTO dto);
        Task<bool> RemoverEventoAsync(Guid id, Guid usuarioId);
    }
}
