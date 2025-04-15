using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Services;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutOrganizerWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public LoginController(
            IUsuarioService usuarioService,
            IAuthService authService,
            IEmailService emailService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
            _emailService = emailService;
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

        public IActionResult CadastrarUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] RequestUsuarioCreate request)
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

                // 📨 Monta a URL para a página de login
                var loginUrl = $"{Request.Scheme}://{Request.Host}/Login";

                // 📨 Envia e-mail de boas-vindas
                await _emailService.EnviarBoasVindasAsync(usuario.Nome, usuario.Email, loginUrl);

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


        [HttpPost]
        public async Task<IActionResult> RedefinirSenha([FromBody] string email)
        {
            try
            {
                var redefinicaoUrlBase = $"{Request.Scheme}://{Request.Host}/Login/NovaSenha";

                await _usuarioService.RedefinirSenhaAsync(email, redefinicaoUrlBase);

                return Json(new { sucesso = true, mensagem = "Instruções de redefinição de senha enviadas para seu e-mail." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = $"Erro: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SalvarNovaSenha([FromBody] RequestNovaSenha request)
        {
            try
            {
                var usuario = _usuarioService.ObterPorToken(request.Token);

                if (usuario == null || usuario.TokenExpiracao == null || usuario.TokenExpiracao < DateTime.UtcNow)
                    return Json(new { sucesso = false, mensagem = "Token inválido ou expirado." });

                usuario.SenhaHash = _authService.GerarHash(request.NovaSenha);
                usuario.TokenRecuperacaoSenha = null;
                usuario.TokenExpiracao = null;

                _usuarioService.Atualizar(usuario);

                return Json(new { sucesso = true, mensagem = "Senha redefinida com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = $"Erro: {ex.Message}" });
            }
        }


        public IActionResult EsqueciSenha()
        {
            return View();
        }


        public IActionResult NovaSenha(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Index");

            var usuario = _usuarioService.ObterPorToken(token);

            if (usuario == null || usuario.TokenExpiracao == null || usuario.TokenExpiracao < DateTime.UtcNow)
            {
                TempData["MensagemErro"] = "O link de redefinição expirou ou é inválido.";
                return RedirectToAction("LinkExpirado");
            }

            ViewBag.Token = token;
            return View();
        }


        public IActionResult LinkExpirado()
        {
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            _authService.RemoverUsuarioDaSessao(HttpContext);
            return RedirectToAction("Index", "Login");
        }


    }
}
