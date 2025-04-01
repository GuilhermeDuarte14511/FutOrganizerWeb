using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Controllers
{
    public class PartidaController : Controller
    {
        private readonly IPartidaService _partidaService;

        public PartidaController(IPartidaService partidaService)
        {
            _partidaService = partidaService;
        }

        [HttpGet("/Partida/Detalhes/{id}")]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            var detalhes = await _partidaService.ObterDetalhesDaPartidaAsync(id);

            if (detalhes == null)
                return NotFound("Partida não encontrada.");

            return View(detalhes);
        }

        [HttpGet]
        public async Task<IActionResult> ObterHistoricoJson(int pagina = 1, int quantidade = 10)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

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
            return Json(dto);
        }
    }
}
