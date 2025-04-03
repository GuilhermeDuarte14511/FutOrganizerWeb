using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = FutOrganizerWeb.Application.DTOs.LoginRequest;

namespace FutOrganizerWebOficial.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;

        public LoginController(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login do usuário com e-mail e senha.
        /// </summary>
        [HttpPost("logar")]
        public IActionResult Logar([FromBody] LoginRequest request)  // Usando o modelo RequestDTO para login
        {
            var usuario = _usuarioService.Logar(request.Email, request.Senha);

            if (usuario != null)
            {
                return Ok(new ResponseDto
                {
                    Sucesso = true,
                    Mensagem = "Login realizado com sucesso!",
                    Data = new { usuario.Id, usuario.Nome }
                });
            }

            return BadRequest(new ResponseDto
            {
                Sucesso = false,
                Mensagem = "Email ou senha inválidos."
            });
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost("criar")]
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

                return Ok(new ResponseDto
                {
                    Sucesso = true,
                    Mensagem = "Usuário cadastrado com sucesso!",
                    Data = new { usuario.Id, usuario.Nome }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao criar usuário: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Envia instruções de redefinição de senha.
        /// </summary>
        [HttpPost("redefinir")]
        public IActionResult RedefinirSenha([FromBody] EmailRequest request)
        {
            try
            {
                _usuarioService.RedefinirSenha(request.Email);
                return Ok(new ResponseDto
                {
                    Sucesso = true,
                    Mensagem = "Instruções de redefinição de senha enviadas para seu e-mail."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Sucesso = false,
                    Mensagem = $"Erro: {ex.Message}"
                });
            }
        }
    }
}
