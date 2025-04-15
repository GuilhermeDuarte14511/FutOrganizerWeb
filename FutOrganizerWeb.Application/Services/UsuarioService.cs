using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using System;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IAuthService authService,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _authService = authService;
            _emailService = emailService;
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

        // Método de Redefinir Senha com token
        public async Task RedefinirSenhaAsync(string email, string redefinicaoUrlBase)
        {
            var usuario = _usuarioRepository.ObterPorEmail(email);
            if (usuario == null)
                throw new Exception("Email não encontrado!");

            // Gera token e define validade
            var token = Guid.NewGuid().ToString("N");
            usuario.TokenRecuperacaoSenha = token;
            usuario.TokenExpiracao = DateTime.UtcNow.AddHours(1);

            _usuarioRepository.Atualizar(usuario);

            // Monta URL completa com o token
            var url = $"{redefinicaoUrlBase}?token={token}";

            // Envia e-mail de recuperação
            await _emailService.EnviarRecuperacaoSenhaAsync(usuario.Nome, usuario.Email, url);
        }

        public Usuario? ObterPorToken(string token)
        {
            return _usuarioRepository.ObterPorToken(token);
        }

        public void Atualizar(Usuario usuario)
        {
            _usuarioRepository.Atualizar(usuario);
        }

        public Usuario ObterPorId(Guid id)
        {
            return _usuarioRepository.ObterPorId(id);
        }

        public void AtualizarDados(Guid id, string nome, string email)
        {
            var usuario = _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            usuario.Nome = nome;
            usuario.Email = email;

            _usuarioRepository.Atualizar(usuario);
        }

        public bool AlterarSenha(Guid id, string senhaAtual, string novaSenha)
        {
            var usuario = _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                return false;

            var senhaValida = _authService.VerificarHash(senhaAtual, usuario.SenhaHash);
            if (!senhaValida)
                return false;

            usuario.SenhaHash = _authService.GerarHash(novaSenha);
            _usuarioRepository.Atualizar(usuario);

            return true;
        }

        public void AtualizarPerfil(Guid id, string nome, string email, string? senhaAtual, string? novaSenha)
        {
            var usuario = _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            usuario.Nome = nome;

            usuario.Email = email;

            if (!string.IsNullOrWhiteSpace(senhaAtual) && !string.IsNullOrWhiteSpace(novaSenha))
            {
                bool senhaValida = _authService.VerificarHash(senhaAtual, usuario.SenhaHash);
                if (!senhaValida)
                    throw new Exception("Senha atual incorreta.");

                usuario.SenhaHash = _authService.GerarHash(novaSenha);
            }

            _usuarioRepository.Atualizar(usuario);
        }



    }
}
