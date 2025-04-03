using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FutOrganizerAPI.Controllers
{
    [ApiController]
    [Route("api/lobby")]
    public class LobbyApiController : ControllerBase
    {
        private readonly IPartidaService _partidaService;
        private readonly IChatService _chatService;

        public LobbyApiController(IPartidaService partidaService, IChatService chatService)
        {
            _partidaService = partidaService;
            _chatService = chatService;
        }

        [HttpGet("{codigo}/jogadores")]
        public async Task<IActionResult> ObterJogadores(string codigo)
        {
            var partida = await _partidaService.ObterPorCodigoAsync(codigo);
            if (partida == null)
                return NotFound();

            var jogadores = partida.JogadoresLobby
                .OrderBy(j => j.DataEntrada)
                .Select(j => new
                {
                    Nome = j.Nome,
                    UltimaAtividade = j.UltimaAtividade
                })
                .ToList();

            return Ok(jogadores);
        }

        [HttpPost("entrar")]
        public async Task<IActionResult> Entrar([FromBody] EntrarLobbyRequest request)
        {
            var jogador = await _partidaService.AdicionarJogadorAoLobbyAsync(request.Codigo, request.Nome);

            var cookieValue = JsonSerializer.Serialize(jogador);
            Response.Cookies.Append($"JogadorLobby_{request.Codigo}", cookieValue, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                IsEssential = true
            });

            return Ok(jogador);
        }

        [HttpDelete("sair")]
        public async Task<IActionResult> Sair([FromQuery] string codigo, [FromQuery] Guid jogadorId)
        {
            await _partidaService.RemoverJogadorAsync(codigo, jogadorId);
            Response.Cookies.Delete($"JogadorLobby_{codigo}");
            return Ok(new { sucesso = true });
        }

        [HttpGet("{codigo}/verificar-sorteio")]
        public async Task<IActionResult> VerificarSorteio(string codigo)
        {
            var sorteio = await _partidaService.ObterSorteioDaPartidaAsync(codigo);
            if (sorteio == null)
                return Ok(new { sorteioRealizado = false });

            var jogadorIdCookie = Request.Cookies[$"JogadorLobby_{codigo}"];
            Guid.TryParse(jogadorIdCookie, out Guid jogadorId);

            return Ok(new
            {
                sorteioRealizado = true,
                sorteio,
                jogadorId = jogadorId
            });
        }

        [HttpGet("{codigo}/mensagens")]
        public async Task<IActionResult> ObterMensagens(string codigo)
        {
            var mensagens = await _chatService.ObterMensagensAsync(codigo);
            return Ok(mensagens);
        }

        [HttpGet("{codigo}/usuarios-online")]
        public IActionResult UsuariosOnline(string codigo)
        {
            var usuariosOnline = FutOrganizerWeb.Hubs.LobbyChatHub.UsuariosConectados
                .Where(kvp => kvp.Value.Sala == codigo)
                .Select(kvp => kvp.Value.Nome)
                .Distinct()
                .ToList();

            return Ok(usuariosOnline);
        }
    }
}
