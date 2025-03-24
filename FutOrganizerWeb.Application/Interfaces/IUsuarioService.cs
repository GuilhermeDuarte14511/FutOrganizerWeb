using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IUsuarioService
    {
        Usuario Logar(string email, string senha);  // Retorna o usuário ou null
        void CriarUsuario(Usuario usuario);         // Não retorna nada, apenas cria
        void RedefinirSenha(string email);          // Envia instruções de redefinição de senha
    }
}
