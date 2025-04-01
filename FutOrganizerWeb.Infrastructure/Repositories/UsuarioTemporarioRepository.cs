using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Infrastructure.Repositories
{
    public class UsuarioTemporarioRepository : IUsuarioTemporarioRepository
    {
        private readonly FutOrganizerDbContext _context;

        public UsuarioTemporarioRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioTemporario?> ObterPorIdAsync(Guid id)
        {
            return await _context.UsuariosTemporarios.FindAsync(id);
        }

        public async Task<UsuarioTemporario?> ObterPorNomeAsync(string nome)
        {
            return await _context.UsuariosTemporarios
                .FirstOrDefaultAsync(u => u.Nome.ToLower() == nome.ToLower());
        }

        public async Task<UsuarioTemporario> CriarAsync(UsuarioTemporario usuario)
        {
            _context.UsuariosTemporarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
