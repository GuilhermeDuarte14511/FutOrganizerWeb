using FutOrganizerWeb.Domain.Entities;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IUsuarioRepository
    {
        Usuario? ObterPorEmail(string email);
        void Adicionar(Usuario usuario);
        bool EmailExiste(string email);
        void Atualizar(Usuario usuario);
        Usuario? ObterPorToken(string token);
        Usuario ObterPorId(Guid id);
    }

}
