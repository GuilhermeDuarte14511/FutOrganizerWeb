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

        public async Task<Partida?> ObterPorCodigoAsync(string codigo)
        {
            return await _context.Partidas
                .Include(p => p.JogadoresLobby)
                .Include(p => p.Sorteios)
                .FirstOrDefaultAsync(p => p.CodigoLobby == codigo);
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CriarPartidaAsync(Partida partida)
        {
            _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarJogadorAsync(JogadorLobby jogador)
        {
            _context.JogadoresLobby.Add(jogador);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverJogadorAsync(string codigo, Guid jogadorId)
        {
            var partida = await ObterPorCodigoAsync(codigo);
            if (partida == null) throw new Exception("Partida não encontrada");

            var jogador = partida.JogadoresLobby.FirstOrDefault(j => j.Id == jogadorId);
            if (jogador != null)
            {
                _context.JogadoresLobby.Remove(jogador);
                await _context.SaveChangesAsync();
            }
        }
    }
}