using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class LobbyController : Controller
{
    private readonly IPartidaService _partidaService;
    private readonly IChatService _chatService;

    public LobbyController(IPartidaService partidaService, IChatService chatService)
    {
        _partidaService = partidaService;
        _chatService = chatService;
    }

    [HttpGet("/Lobby/{codigo}")]
    public async Task<IActionResult> EntrarConvidado(string codigo)
    {
        var partida = await _partidaService.ObterPorCodigoAsync(codigo);
        if (partida == null)
            return NotFound();

        var jogadorIdCookie = Request.Cookies[$"JogadorLobby_{codigo}"];
        if (!string.IsNullOrEmpty(jogadorIdCookie))
        {
            var jogadores = partida.JogadoresLobby
                .OrderBy(j => j.DataEntrada)
                .Select(j => j.Nome)
                .ToList();

            var viewModel = new SorteioLobbyViewModel
            {
                Codigo = codigo,
                Jogadores = jogadores
            };

            return View("VisualizarSala", viewModel); // já entrou antes, exibe sala
        }

        var vm = new SorteioLobbyViewModel
        {
            Codigo = codigo,
            Jogadores = partida.JogadoresLobby
                .OrderBy(j => j.DataEntrada)
                .Select(j => j.Nome)
                .ToList()
        };

        return View("EntrarConvidado", vm);
    }

    [HttpPost]
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


    [HttpGet("/Lobby/Jogadores")]
    public async Task<IActionResult> Jogadores(string codigo)
    {
        var partida = await _partidaService.ObterPorCodigoAsync(codigo);
        if (partida == null)
            return NotFound();

        var jogadores = partida.JogadoresLobby
            .OrderBy(j => j.DataEntrada)
            .Select(j => j.Nome)
            .ToList();

        return Json(jogadores);
    }

    [HttpDelete("/Lobby/Sair")]
    public async Task<IActionResult> RemoverJogador([FromQuery] string codigo, [FromQuery] Guid jogadorId)
    {
        await _partidaService.RemoverJogadorAsync(codigo, jogadorId);
        Response.Cookies.Delete($"JogadorLobby_{codigo}");
        return Ok();
    }

    [HttpGet("/Lobby/VerificarSorteio")]
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


    [HttpGet("/Lobby/Mensagens")]
    public async Task<IActionResult> Mensagens(string codigo)
    {
        var mensagens = await _chatService.ObterMensagensAsync(codigo);
        return Json(mensagens);
    }


}
