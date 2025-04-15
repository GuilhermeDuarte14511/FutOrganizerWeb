using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Domain.Interfaces
{
    public interface IUsuarioService
    {
        Usuario Logar(string email, string senha); 
        void CriarUsuario(Usuario usuario);        
        Task RedefinirSenhaAsync(string email, string redefinicaoUrlBase);
        Usuario? ObterPorToken(string token);
        void Atualizar(Usuario usuario);
        Usuario ObterPorId(Guid id);
        void AtualizarDados(Guid id, string nome, string email);
        bool AlterarSenha(Guid id, string senhaAtual, string novaSenha);
        void AtualizarPerfil(Guid id, string nome, string email, string? senhaAtual, string? novaSenha);

    }
}
