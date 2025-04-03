using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FutOrganizerWebOficial.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidaController : ControllerBase
    {
        private readonly IPartidaService _partidaService;

        public PartidaController(IPartidaService partidaService)
        {
            _partidaService = partidaService;
        }

        /// <summary>
        /// Retorna os detalhes de uma partida por ID.
        /// </summary>
        [HttpGet("detalhes/{id}")]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            var detalhes = await _partidaService.ObterDetalhesDaPartidaAsync(id);

            if (detalhes == null)
                return NotFound(new { sucesso = false, mensagem = "Partida não encontrada." });

            return Ok(detalhes);
        }

        /// <summary>
        /// Retorna o histórico de partidas de um usuário.
        /// </summary>
        [HttpGet("historico")]
        public async Task<IActionResult> ObterHistoricoJson([FromQuery] Guid usuarioId, int pagina = 1, int quantidade = 10)
        {
            var partidas = await _partidaService.ObterPartidasPaginadasPorUsuarioAsync(usuarioId, pagina, quantidade);

            var tarefas = partidas.Select(async p =>
            {
                var endereco = await AppHelper.ObterEnderecoPorCoordenadasAsync(p.Latitude ?? 0, p.Longitude ?? 0);

                return new HistoricoPartidaDTO
                {
                    Nome = $"Partida - {p.DataHora:dd/MM/yyyy HH:mm}",
                    Local = endereco,
                    Data = p.DataHora,
                    Latitude = p.Latitude ?? 0.0,
                    Longitude = p.Longitude ?? 0.0,
                    PartidaId = p.Id
                };
            });

            var dto = await Task.WhenAll(tarefas);
            return Ok(dto);
        }
    }
}
