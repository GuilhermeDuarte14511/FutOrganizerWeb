using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Repositories
{
    public class PartidaRepository : IPartidaRepository
    {
        private readonly FutOrganizerDbContext _context;

        public PartidaRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Partidas
                .Include(p => p.Sorteios)
                .Where(p => p.UsuarioCriadorId == usuarioId)
                .OrderByDescending(p => p.DataHora)
                .ToListAsync();
        }

        public async Task<Partida?> ObterPartidaComSorteioAsync(Guid partidaId)
        {
            return await _context.Partidas
                .Include(p => p.Sorteios)
                    .ThenInclude(s => s.Times)
                        .ThenInclude(t => t.Jogadores)
                .Include(p => p.Sorteios)
                    .ThenInclude(s => s.Times)
                        .ThenInclude(t => t.Goleiro)
                .FirstOrDefaultAsync(p => p.Id == partidaId);
        }
    }
}
