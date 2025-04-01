using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LobbyController : Controller
{
    private readonly IPartidaService _partidaService;

    public LobbyController(IPartidaService partidaService)
    {
        _partidaService = partidaService;
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
        var jogadorId = await _partidaService.AdicionarJogadorAoLobbyAsync(request.Codigo, request.Nome);

        // Salva cookie por 7 dias
        Response.Cookies.Append($"JogadorLobby_{request.Codigo}", jogadorId.ToString(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            IsEssential = true
        });

        return Ok(jogadorId);
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


}
