using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Application.Interfaces.Services
{
    public interface IPartidaService
    {
        Task<List<HistoricoPartidaDto>> ObterHistoricoAsync(Guid usuarioId, int pagina = 1, int quantidade = 10);
        Task<PartidaLobbyDto?> ObterPorCodigoAsync(string codigo);
        Task<string?> CriarSalaAsync(CriarSalaRequest request);
        Task<bool> CriarSorteioAsync(SorteioRequest request);
        Task<List<SalaViewModel>> ObterSalasAsync(Guid usuarioId, int pagina = 1, int tamanhoPagina = 10);
        Task<JogadorDTO?> EntrarNaSalaComoLogadoAsync(string codigo, JogadorDTO jogador);
        Task<List<MensagemChatDTO>> ObterMensagensChatAsync(string codigo);
        Task<MensagemChatDTO?> EnviarMensagemChatAsync(string codigo, string nome, string conteudo);
        Task AtualizarAtividadeAsync(string codigo, string nomeJogador);
        Task<List<JogadorDTO>> ObterJogadoresAsync(string codigo);
        Task<List<string>> ObterUsuariosOnlineAsync(string codigo);

    }
}