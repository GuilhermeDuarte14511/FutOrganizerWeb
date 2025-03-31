using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IPartidaService
    {
        Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId);
        Task<DetalhesPartidaDTO> ObterDetalhesDaPartidaAsync(Guid partidaId);

    }
}
