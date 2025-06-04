using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Repositories
{
    public class SorteioRepository : ISorteioRepository
    {
        private readonly FutOrganizerDbContext _context;

        public SorteioRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CriarPartidaComSorteioAsync(Partida partida)
        {
            await _context.Partidas.AddAsync(partida);
            await _context.SaveChangesAsync();
            return partida.Sorteios.First().Id;
        }

        public async Task<Sorteio?> ObterSorteioComTimesAsync(Guid sorteioId)
        {
            return await _context.Sorteios
                .Include(s => s.Partida)
                .Include(s => s.Times)
                    .ThenInclude(t => t.Jogadores)
                .Include(s => s.Times)
                    .ThenInclude(t => t.Goleiro)
                .FirstOrDefaultAsync(s => s.Id == sorteioId);
        }


        public async Task AtualizarSorteioAsync(Sorteio sorteio)
        {
            var sorteioExistente = await _context.Sorteios
                .Include(s => s.Times)
                    .ThenInclude(t => t.Jogadores)
                .Include(s => s.Times)
                    .ThenInclude(t => t.Goleiro)
                .FirstOrDefaultAsync(s => s.Id == sorteio.Id);

            if (sorteioExistente == null)
                throw new Exception("Sorteio não encontrado.");

            // Remove Jogadores e Goleiros de forma segura
            var timesAntigos = sorteioExistente.Times.ToList(); // evita modificar durante iteração

            foreach (var time in timesAntigos)
            {
                if (time.Jogadores?.Any() == true)
                    _context.Jogadores.RemoveRange(time.Jogadores.ToList());

                if (time.Goleiro != null)
                    _context.Goleiros.Remove(time.Goleiro);
            }

            _context.Times.RemoveRange(timesAntigos);

            // Desanexar antigo para evitar conflito
            _context.Entry(sorteioExistente).State = EntityState.Detached;

            // Adicionar novamente os novos dados
            sorteio.Id = sorteioExistente.Id; // manter o ID original
            _context.Sorteios.Update(sorteio);

            await _context.SaveChangesAsync();
        }


        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<Guid> ResortearAsync(Guid sorteioId, List<TimeRequest> novosTimes)
        {
            var sorteioAntigo = await _context.Sorteios
                .Include(s => s.Times).ThenInclude(t => t.Jogadores)
                .Include(s => s.Times).ThenInclude(t => t.Goleiro)
                .FirstOrDefaultAsync(s => s.Id == sorteioId);

            if (sorteioAntigo == null)
                throw new Exception("Sorteio não encontrado.");

            var partidaId = sorteioAntigo.PartidaId;

            // Conta quantos sorteios já existem para essa partida
            var totalSorteios = await _context.Sorteios
                .CountAsync(s => s.PartidaId == partidaId);

            // Remove Jogadores, Goleiros e Times do sorteio antigo
            foreach (var time in sorteioAntigo.Times.ToList())
            {
                if (time.Jogadores?.Any() == true)
                    _context.Jogadores.RemoveRange(time.Jogadores.ToList());

                if (time.Goleiro != null)
                    _context.Goleiros.Remove(time.Goleiro);
            }

            _context.Times.RemoveRange(sorteioAntigo.Times);
            _context.Sorteios.Remove(sorteioAntigo);
            await _context.SaveChangesAsync();

            // Cria o novo sorteio com novos times
            var novoSorteio = new Sorteio
            {
                Nome = $"Sorteio {totalSorteios + 1}",
                PartidaId = partidaId,
                Data = DateTime.UtcNow.AddHours(-3),
                Times = novosTimes.Select(t => new Time
                {
                    Nome = t.Nome,
                    CorHex = t.CorHex,
                    Jogadores = t.Jogadores.Select(j => new Jogador { Nome = j }).ToList(),
                    Goleiro = string.IsNullOrWhiteSpace(t.Goleiro) ? null : new Goleiro { Nome = t.Goleiro }
                }).ToList()
            };

            _context.Sorteios.Add(novoSorteio);
            await _context.SaveChangesAsync();

            return novoSorteio.Id;
        }

        public async Task RemoverPartidaCompletaAsync(Guid partidaId)
        {
            var partida = await _context.Partidas
                .Include(p => p.Sorteios)
                    .ThenInclude(s => s.Times)
                        .ThenInclude(t => t.Jogadores)
                .Include(p => p.Sorteios)
                    .ThenInclude(s => s.Times)
                        .ThenInclude(t => t.Goleiro)
                .FirstOrDefaultAsync(p => p.Id == partidaId);

            if (partida != null)
            {
                _context.Partidas.Remove(partida);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid> CriarSorteioParaPartidaExistenteAsync(Sorteio sorteio)
        {
            await _context.Sorteios.AddAsync(sorteio);
            await _context.SaveChangesAsync();
            return sorteio.Id;
        }




    }
}
