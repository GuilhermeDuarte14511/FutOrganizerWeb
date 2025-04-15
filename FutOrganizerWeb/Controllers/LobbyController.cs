using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Controllers;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class LobbyController : BaseController
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

        var usuarioId = ObterUsuarioLogado();

        // ✅ Redireciona se o usuário for o criador da partida
        if (usuarioId != Guid.Empty && partida.UsuarioCriadorId == usuarioId)
        {
            return RedirectToAction("Index", "Sorteio", new { codigo });
        }

        try
        {
            var usuarioNome = HttpContext.Session.GetString("UsuarioNome") ?? "Jogador";
            var jogadorExistente = partida.JogadoresLobby.FirstOrDefault(j => j.UsuarioAutenticadoId == usuarioId);
            JogadorDTO jogadorDTO;

            if (jogadorExistente != null)
            {
                jogadorDTO = new JogadorDTO
                {
                    Id = jogadorExistente.Id,
                    Nome = jogadorExistente.Nome
                };
            }
            else
            {
                var novoJogador = new JogadorLobby
                {
                    Nome = usuarioNome,
                    UsuarioAutenticadoId = usuarioId,
                    PartidaId = partida.Id,
                    DataEntrada = DateTime.UtcNow.AddHours(-3),
                    UltimaAtividade = DateTime.UtcNow.AddHours(-3)
                };

                await _partidaService.AdicionarJogadorAoLobbyAsync(codigo, usuarioNome, null, usuarioId);

                jogadorDTO = new JogadorDTO
                {
                    Id = novoJogador.Id,
                    Nome = novoJogador.Nome
                };
            }

            // 🍪 Salva ID e Nome em cookies
            Response.Cookies.Append($"JogadorLobbyId_{codigo}", jogadorDTO.Id.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true
            });

            Response.Cookies.Append($"JogadorLobbyNome_{codigo}", jogadorDTO.Nome, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true
            });

            return View("VisualizarSala", new SorteioLobbyViewModel
            {
                Codigo = codigo,
                Jogadores = partida.JogadoresLobby.OrderBy(j => j.DataEntrada).Select(j => j.Nome).ToList(),
                NomeDoJogador = jogadorDTO.Nome,
                JogadorId = jogadorDTO.Id,
                UsuarioAutenticadoId = usuarioId != Guid.Empty ? usuarioId : null
            });
        }
        catch
        {
            // continua como convidado
        }

        var jogadorIdCookie = Request.Cookies[$"JogadorLobbyId_{codigo}"];
        var jogadorNomeCookie = Request.Cookies[$"JogadorLobbyNome_{codigo}"];

        if (!string.IsNullOrEmpty(jogadorIdCookie) && Guid.TryParse(jogadorIdCookie, out Guid jogadorId))
        {
            var jogador = partida.JogadoresLobby.FirstOrDefault(j => j.Id == jogadorId);
            if (jogador != null)
            {
                return View("VisualizarSala", new SorteioLobbyViewModel
                {
                    Codigo = codigo,
                    Jogadores = partida.JogadoresLobby.OrderBy(j => j.DataEntrada).Select(j => j.Nome).ToList(),
                    NomeDoJogador = jogador.Nome,
                    JogadorId = jogador.Id,
                    UsuarioAutenticadoId = jogador.UsuarioAutenticadoId
                });
            }

            Response.Cookies.Delete($"JogadorLobbyId_{codigo}");
            Response.Cookies.Delete($"JogadorLobbyNome_{codigo}");
        }

        return View("EntrarConvidado", new SorteioLobbyViewModel
        {
            Codigo = codigo,
            Jogadores = partida.JogadoresLobby.OrderBy(j => j.DataEntrada).Select(j => j.Nome).ToList()
        });
    }




    [HttpPost]
    public async Task<IActionResult> Entrar([FromBody] EntrarLobbyRequest request)
    {
        // Validação básica do email
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email é obrigatório.");

        var jogador = await _partidaService.AdicionarJogadorAoLobbyAsync(
            request.Codigo,
            request.Nome,
            request.Email,
            request.UsuarioId
        );

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
            .Select(j => new
            {
                Nome = j.Nome,
                UltimaAtividade = j.UltimaAtividade,
                Identificador = j.UsuarioAutenticadoId.HasValue && j.UsuarioAutenticadoId != Guid.Empty
                    ? j.UsuarioAutenticadoId.ToString()
                    : j.Id.ToString()
            })
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

    [HttpGet("/Lobby/UsuariosOnline")]
    public IActionResult UsuariosOnline(string codigo)
    {
        var usuariosOnline = FutOrganizerWeb.Hubs.LobbyChatHub.UsuariosConectados
            .Where(kvp => kvp.Value.Sala == codigo)
            .Select(kvp => kvp.Value.Identificador)
            .Distinct()
            .ToList();

        return Ok(usuariosOnline);
    }
}
