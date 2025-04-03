using FutOrganizerWeb.Application.Interfaces.Repositories;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly FutOrganizerDbContext _context;

        public ChatRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarMensagemAsync(MensagemChat mensagem)
        {
            _context.Add(mensagem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MensagemChat>> ObterMensagensPorCodigoSalaAsync(string codigoSala)
        {
            return await _context.MensagensChat
                .Include(m => m.Partida)
                .Where(m => m.Partida!.CodigoLobby == codigoSala)
                .ToListAsync();
        }
    }
}
