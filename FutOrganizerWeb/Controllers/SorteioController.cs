﻿using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Helpers;
using FutOrganizerWeb.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FutOrganizerWeb.Controllers
{
    public class SorteioController : BaseController
    {
        private readonly ISorteioService _sorteioService;
        private readonly IPartidaService _partidaService;
        private readonly IUsuarioService _usuarioService;

        public SorteioController(
            ISorteioService sorteioService,
            IPartidaService partidaService,
            IUsuarioService usuarioService)
        {
            _sorteioService = sorteioService;
            _partidaService = partidaService;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Route("Sorteio")]
        public async Task<IActionResult> Index(string? codigo = null, Guid? eventoId = null)
        {
            if (string.IsNullOrEmpty(codigo))
                return View(); // fluxo normal (sem lobby)

            var partida = await _partidaService.ObterPorCodigoAsync(codigo);
            if (partida == null) return NotFound();

            var jogadores = partida.JogadoresLobby.Select(j => j.Nome).ToList();

            var usuarioId = ObterUsuarioLogado();
            var usuario = _usuarioService.ObterPorId(usuarioId);

            var viewModel = new SorteioLobbyViewModel
            {
                Codigo = codigo,
                Jogadores = jogadores,
                UsuarioAutenticadoId = usuarioId,
                NomeDoJogador = usuario?.Nome ?? "Admin",
                EventoId = eventoId // novo campo na ViewModel
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
                sorteioId = await _sorteioService.CriarSorteioParaLobbyAsync(request.CodigoLobby, request);

                var nomeAdmin = HttpContext.Session.GetString("UsuarioNome") ?? "Administrador";
                await _partidaService.AdicionarJogadorAoLobbyAsync(request.CodigoLobby, nomeAdmin, null, usuarioId);
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
        public async Task<IActionResult> CriarSala(DateTime DataHora, string Local, string Latitude, string Longitude, Guid? eventoId = null)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

            var codigoLobby = Guid.NewGuid().ToString("N")[..6].ToUpper();

            var latitudeParsed = double.TryParse(Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ? lat : (double?)null;
            var longitudeParsed = double.TryParse(Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng) ? lng : (double?)null;

            var novaPartida = new Partida
            {
                DataHora = DataHora,
                Local = Local,
                Latitude = latitudeParsed,
                Longitude = longitudeParsed,
                CodigoLobby = codigoLobby,
                UsuarioCriadorId = usuarioId,
                EventoId = eventoId
            };

            await _partidaService.CriarPartidaAsync(novaPartida);

            var nomeAdmin = HttpContext.Session.GetString("UsuarioNome") ?? "Administrador";
            await _partidaService.AdicionarJogadorAoLobbyAsync(codigoLobby, nomeAdmin, null, usuarioId);

            // ✅ Se tiver eventoId → retorna URL completa para redirecionamento
            if (eventoId.HasValue && eventoId != Guid.Empty)
            {
                var url = Url.Action("Index", "Sorteio", new { codigo = codigoLobby, eventoId }, Request.Scheme);
                return Json(new
                {
                    sucesso = true,
                    redirectUrl = url
                });
            }

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
            var usuarioId = ObterUsuarioLogado();
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
