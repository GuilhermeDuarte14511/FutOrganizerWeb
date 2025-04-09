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
        Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina);
        Task<DetalhesPartidaDTO> ObterDetalhesDaPartidaAsync(Guid partidaId);
        Task<Partida?> ObterPorCodigoAsync(string codigo);
        Task<JogadorDTO> AdicionarJogadorAoLobbyAsync(string codigo, string nomeJogador, string? email = null, Guid? usuarioId = null);
        Task CriarPartidaAsync(Partida partida);
        Task RemoverJogadorAsync(string codigo, Guid jogadorId);
        Task<List<Partida>> ObterPartidasPaginadasPorUsuarioAsync(Guid usuarioId, int page, int pageSize);
        Task<SorteioDTO?> ObterSorteioDaPartidaAsync(string codigo);
        Task AtualizarUltimaAtividadeAsync(string codigo, string nomeJogador);


    }
}
