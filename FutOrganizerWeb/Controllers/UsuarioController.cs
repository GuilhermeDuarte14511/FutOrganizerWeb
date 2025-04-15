using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Controllers;
using FutOrganizerWeb.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class UsuarioController : BaseController
{
    private readonly IUsuarioService _usuarioService;
    private readonly IAuthService _authService;

    public UsuarioController(IUsuarioService usuarioService, IAuthService authService)
    {
        _usuarioService = usuarioService;
        _authService = authService;
    }

    [HttpGet]
    public IActionResult MeusDados()
    {
        var usuarioId = ObterUsuarioLogado();
        var usuario = _usuarioService.ObterPorId(usuarioId);

        if (usuario == null)
            return RedirectToAction("Index", "Login");

        return View(usuario);
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes([FromBody] RequestSalvarAlteracoes request)
    {
        var usuarioId = ObterUsuarioLogado();
        var mensagens = new List<object>();

        try
        {
            _usuarioService.AtualizarPerfil(usuarioId, request.Nome, request.Email, request.SenhaAtual, request.NovaSenha);
            mensagens.Add(new { sucesso = true, mensagem = "Alterações salvas com sucesso!" });
        }
        catch (Exception ex)
        {
            mensagens.Add(new { sucesso = false, mensagem = ex.Message });
        }

        return Json(mensagens);
    }

}
