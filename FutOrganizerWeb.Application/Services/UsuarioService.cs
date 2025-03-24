using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Interfaces_Repositories;
using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;

        public UsuarioService(IUsuarioRepository usuarioRepository, IAuthService authService)
        {
            _usuarioRepository = usuarioRepository;
            _authService = authService;
        }

        // Método de Login
        public Usuario Logar(string email, string senha)
        {
            var usuario = _usuarioRepository.ObterPorEmail(email);
            if (usuario != null && _authService.VerificarHash(senha, usuario.SenhaHash))
            {
                return usuario; // Retorna o usuário se a senha for válida
            }
            return null; // Retorna null se falhar
        }

        // Método de Cadastro de Novo Usuário
        public void CriarUsuario(Usuario usuario)
        {
            if (_usuarioRepository.EmailExiste(usuario.Email))
                throw new Exception("Email já cadastrado!");

            _usuarioRepository.Adicionar(usuario); // Cria o usuário no banco
        }

        // Método de Redefinir Senha
        public void RedefinirSenha(string email)
        {
            var usuario = _usuarioRepository.ObterPorEmail(email);
            if (usuario == null)
                throw new Exception("Email não encontrado!");

        }
    }
}
