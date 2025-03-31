using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FutOrganizerDbContext _context;

        public UsuarioRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        // Buscar usuário por email
        public Usuario? ObterPorEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email);
        }

        // Adicionar um novo usuário
        public void Adicionar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        // Verificar se o email já está registrado
        public bool EmailExiste(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
        }
    }
}
