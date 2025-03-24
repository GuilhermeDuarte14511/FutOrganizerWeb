using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Application.Interfaces_Repositories
{
    public interface IUsuarioRepository
    {
        // Obter o usuário por email
        Usuario? ObterPorEmail(string email);

        // Adicionar um novo usuário
        void Adicionar(Usuario usuario);

        // Verificar se o email já existe
        bool EmailExiste(string email);
    }
}
