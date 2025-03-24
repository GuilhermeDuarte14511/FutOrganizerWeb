using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FutOrganizerWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;

        public LoginController(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }

        // Página de Login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logar(string email, string senha)
        {
            var usuario = _usuarioService.Logar(email, senha);

            if (usuario != null)
            {
                _authService.SalvarUsuarioNaSessao(HttpContext, usuario);

                return Json(new
                {
                    sucesso = true,
                    idUsuario = usuario.Id,
                    nome = usuario.Nome,
                    mensagem = "Login realizado com sucesso!"
                });
            }

            return Json(new { sucesso = false, mensagem = "Email ou senha inválidos." });
        }


        [HttpPost]
        public IActionResult CriarUsuario([FromBody] RequestUsuarioCreate request)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nome = request.Nome,
                    Email = request.Email,
                    SenhaHash = _authService.GerarHash(request.Senha)
                };

                _usuarioService.CriarUsuario(usuario);

                return Json(new
                {
                    sucesso = true,
                    idUsuario = usuario.Id,
                    nome = usuario.Nome,
                    mensagem = "Usuário cadastrado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = $"Erro ao criar usuário: {ex.Message}" });
            }
        }

        // Ação para redefinir senha
        [HttpPost]
        public IActionResult RedefinirSenha(string email)
        {
            try
            {
                _usuarioService.RedefinirSenha(email);
                return Json(new { sucesso = true, mensagem = "Instruções de redefinição de senha enviadas para seu e-mail." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = $"Erro: {ex.Message}" });
            }
        }
    }
}
