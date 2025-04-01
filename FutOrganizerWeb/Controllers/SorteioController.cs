using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FutOrganizerWeb.Controllers
{
    public class SorteioController : Controller
    {
        private readonly ISorteioService _sorteioService;
        private readonly IPartidaService _partidaService;

        public SorteioController(ISorteioService sorteioService, IPartidaService partidaService)
        {
            _sorteioService = sorteioService;
            _partidaService = partidaService;
        }

        public async Task<IActionResult> Index(string? codigo = null)
        {   
            if (string.IsNullOrEmpty(codigo))
                return View(); // fluxo normal (sem lobby)

            var partida = await _partidaService.ObterPorCodigoAsync(codigo);
            if (partida == null) return NotFound();

            var jogadores = partida.JogadoresLobby.Select(j => j.Nome).ToList();

            var viewModel = new SorteioLobbyViewModel
            {
                Codigo = codigo,
                Jogadores = jogadores
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] SorteioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

            request.UsuarioCriadorId = usuarioId;

            var sorteioId = await _sorteioService.CriarSorteioAsync(request);
            return Ok(new { sucesso = true, sorteioId });
        }

        [HttpPost]
        public async Task<IActionResult> Resortear([FromBody] ResortearRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var sorteioId = await _sorteioService.ResortearAsync(request.SorteioId, request.Times);
            return Ok(new { sucesso = true, sorteioId });
        }

        [HttpGet]
        public IActionResult Historico()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarSala(DateTime DataHora, string Local, string Latitude, string Longitude)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

            var codigoLobby = Guid.NewGuid().ToString("N")[..6].ToUpper();

            // Força a leitura usando cultura que aceita ponto
            var latitudeParsed = double.TryParse(Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ? lat : (double?)null;
            var longitudeParsed = double.TryParse(Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng) ? lng : (double?)null;

            var novaPartida = new Partida
            {
                DataHora = DateTime.UtcNow.AddHours(-3),
                Local = Local,
                Latitude = latitudeParsed,
                Longitude = longitudeParsed,
                CodigoLobby = codigoLobby,
                UsuarioCriadorId = usuarioId
            };

            await _partidaService.CriarPartidaAsync(novaPartida);

            return Redirect($"/Sorteio?codigo={codigoLobby}");
        }

        [HttpGet]
        public async Task<IActionResult> MinhasSalas()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

            var partidas = await _partidaService.ObterPartidasPorUsuarioAsync(usuarioId);

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

            return View(salas);
        }
    }
}