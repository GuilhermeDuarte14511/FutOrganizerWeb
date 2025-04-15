using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FutOrganizerWeb.Controllers
{
    public class SorteioController : BaseController
    {
        private readonly ISorteioService _sorteioService;
        private readonly IPartidaService _partidaService;

        public SorteioController(ISorteioService sorteioService, IPartidaService partidaService)
        {
            _sorteioService = sorteioService;
            _partidaService = partidaService;
        }

        [HttpGet]
        [Route("Sorteio")]
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

            Guid? sorteioId;

            if (!string.IsNullOrWhiteSpace(request.CodigoLobby))
            {
                // Criação do sorteio em uma partida já existente (modo lobby)
                sorteioId = await _sorteioService.CriarSorteioParaLobbyAsync(request.CodigoLobby, request);
            }
            else
            {
                // Criação de nova partida e sorteio (modo normal)
                sorteioId = await _sorteioService.CriarSorteioAsync(request);
            }

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

        [HttpGet]
        public IActionResult MinhasSalas()
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
        [Route("Sorteio/ObterSalas")]
        public async Task<IActionResult> ObterSalas(int pagina = 1, int tamanhoPagina = 10)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
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

            return Json(salas);
        }

        [HttpGet]
        public async Task<IActionResult> MinhasParticipacoes()
        {
            var usuarioId = ObterUsuarioLogado(); // método já existente
            var salas = await _partidaService.ObterSalasOndeParticipeiAsync(usuarioId);

            foreach (var partida in salas)
            {
                if (string.IsNullOrWhiteSpace(partida.Local) &&
                    partida.Latitude.HasValue && partida.Longitude.HasValue)
                {
                    var endereco = await AppHelper.ObterEnderecoPorCoordenadasAsync(partida.Latitude.Value, partida.Longitude.Value);
                    partida.Local = endereco;
                }
            }

            return View(salas);
        }



    }
}