using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ConfrontoRepository : IConfrontoRepository
{
    private readonly FutOrganizerDbContext _context;

    public ConfrontoRepository(FutOrganizerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Confronto confronto)
    {
        await _context.AddAsync(confronto);
    }

    public async Task<List<Confronto>> GetBySorteioAsync(Guid sorteioId)
    {
        return await _context.Confrontos
            .Where(c => c.SorteioId == sorteioId)
            .Include(c => c.TimeA).ThenInclude(t => t.Jogadores)
            .Include(c => c.TimeB).ThenInclude(t => t.Jogadores)
            .Include(c => c.Gols)
            .Include(c => c.Assistencias)
            .ToListAsync();
    }


    public async Task<Confronto?> GetByIdAsync(Guid id)
    {
        return await _context.Confrontos
            .Include(c => c.TimeA).ThenInclude(t => t.Jogadores)
            .Include(c => c.TimeB).ThenInclude(t => t.Jogadores)
            .Include(c => c.Gols)
            .Include(c => c.Assistencias)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task AddGolAsync(Gol gol)
    {
        await _context.Gols.AddAsync(gol);
    }

    public async Task AddAssistenciaAsync(Assistencia assistencia)
    {
        await _context.Assistencias.AddAsync(assistencia);
    }
}
