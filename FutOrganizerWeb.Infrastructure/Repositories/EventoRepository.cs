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
    public class EventoRepository : IEventoRepository
    {
        private readonly FutOrganizerDbContext _context;

        public EventoRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<Evento?> ObterPorIdAsync(Guid id)
        {
            return await _context.Eventos
                .Include(e => e.Partidas)
                .ThenInclude(p => p.Sorteios)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Evento>> ObterTodosDoUsuarioAsync(Guid usuarioId)
        {
            return await _context.Eventos
                .Include(e => e.Partidas)
                .Where(e => e.UsuarioCriadorId == usuarioId)
                .OrderByDescending(e => e.Data)
                .ToListAsync();
        }


        public async Task CriarAsync(Evento evento)
        {
            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Evento evento)
        {
            _context.Eventos.Update(evento);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Guid id)
        {
            var evento = await ObterPorIdAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
            }
        }
    }
}
