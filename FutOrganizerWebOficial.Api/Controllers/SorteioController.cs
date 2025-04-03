using System.Globalization;
using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FutOrganizerWebOficial.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SorteioController : ControllerBase
    {
        private readonly ISorteioService _sorteioService;
        private readonly IPartidaService _partidaService;

        public SorteioController(ISorteioService sorteioService, IPartidaService partidaService)
        {
            _sorteioService = sorteioService;
            _partidaService = partidaService;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> Criar([FromBody] SorteioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            if (request.UsuarioCriadorId == Guid.Empty)
                return Unauthorized("Usuário não autenticado.");

            Guid? sorteioId;

            if (!string.IsNullOrWhiteSpace(request.CodigoLobby))
            {
                sorteioId = await _sorteioService.CriarSorteioParaLobbyAsync(request.CodigoLobby, request);
            }
            else
            {
                sorteioId = await _sorteioService.CriarSorteioAsync(request);
            }

            return Ok(new { sucesso = true, sorteioId });
        }

        [HttpPost("resortear")]
        public async Task<IActionResult> Resortear([FromBody] ResortearRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var sorteioId = await _sorteioService.ResortearAsync(request.SorteioId, request.Times);
            return Ok(new { sucesso = true, sorteioId });
        }

        [HttpPost("criar-sala")]
        public async Task<IActionResult> CriarSala([FromBody] CriarSalaRequest dto)
        {
            if (dto.UsuarioId == Guid.Empty)
                return Unauthorized("Usuário não autenticado.");

            var codigoLobby = Guid.NewGuid().ToString("N")[..6].ToUpper();

            var latitudeParsed = double.TryParse(dto.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ? lat : (double?)null;
            var longitudeParsed = double.TryParse(dto.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng) ? lng : (double?)null;

            var novaPartida = new Partida
            {
                DataHora = DateTime.UtcNow.AddHours(-3),
                Local = dto.Local,
                Latitude = latitudeParsed,
                Longitude = longitudeParsed,
                CodigoLobby = codigoLobby,
                UsuarioCriadorId = dto.UsuarioId
            };

            await _partidaService.CriarPartidaAsync(novaPartida);

            return Ok(new { sucesso = true, codigoLobby });
        }

        [HttpGet("salas")]
        public async Task<IActionResult> ObterSalas([FromQuery] Guid usuarioId, int pagina = 1, int tamanhoPagina = 10)
        {
            if (usuarioId == Guid.Empty)
                return Unauthorized("Usuário não autenticado.");

            var partidas = await _partidaService.ObterPartidasPorUsuarioAsync(usuarioId, pagina, tamanhoPagina);

            var salas = partidas.Select(p => new SalaViewModel
            {
                PartidaId = p.Id,
                Codigo = p.CodigoLobby ?? "",
                DataHora = p.DataHora,
                Local = p.Local ?? "Não informado",
                Finalizada = p.Sorteios.Any(),
                Latitude = p.Latitude ?? 0.0,
                Longitude = p.Longitude ?? 0.0
            }).ToList();

            return Ok(salas);
        }
    }
}
