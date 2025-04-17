using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;

public class ConfrontoService : IConfrontoService
{
    private readonly IConfrontoRepository _repository;

    public ConfrontoService(IConfrontoRepository repository)
    {
        _repository = repository;
    }

    public async Task CriarConfrontoAsync(Guid sorteioId, Guid timeAId, Guid timeBId)
    {
        var confronto = new Confronto
        {
            SorteioId = sorteioId,
            TimeAId = timeAId,
            TimeBId = timeBId
        };

        await _repository.AddAsync(confronto);
        await _repository.SaveAsync();
    }

    public async Task<List<ConfrontoDTO>> ObterConfrontosPorSorteioAsync(Guid sorteioId)
    {
        var confrontos = await _repository.GetBySorteioAsync(sorteioId);

        return confrontos.Select(c => new ConfrontoDTO
        {
            Id = c.Id,
            TimeA = c.TimeA?.Nome ?? "",
            TimeB = c.TimeB?.Nome ?? "",
            GolsA = c.GolsTimeA,
            GolsB = c.GolsTimeB,

            JogadoresTimeA = c.TimeA?.Jogadores.Select(j => new JogadorDTO
            {
                Id = j.Id,
                Nome = j.Nome,
                Gols = c.Gols.Count(g => g.JogadorId == j.Id),
                Assistencias = c.Assistencias.Count(a => a.JogadorId == j.Id)
            }).ToList() ?? new List<JogadorDTO>(),

            JogadoresTimeB = c.TimeB?.Jogadores.Select(j => new JogadorDTO
            {
                Id = j.Id,
                Nome = j.Nome,
                Gols = c.Gols.Count(g => g.JogadorId == j.Id),
                Assistencias = c.Assistencias.Count(a => a.JogadorId == j.Id)
            }).ToList() ?? new List<JogadorDTO>()
        }).ToList();
    }

    public async Task AdicionarGolAsync(Guid confrontoId, Guid jogadorId)
    {
        var confronto = await _repository.GetByIdAsync(confrontoId);
        if (confronto is null) return;

        var gol = new Gol
        {
            Id = Guid.NewGuid(),
            ConfrontoId = confrontoId,
            JogadorId = jogadorId,
            Minuto = 0
        };

        if (confronto.TimeA?.Jogadores.Any(j => j.Id == jogadorId) == true)
            confronto.GolsTimeA++;
        else if (confronto.TimeB?.Jogadores.Any(j => j.Id == jogadorId) == true)
            confronto.GolsTimeB++;

        await _repository.AddGolAsync(gol);
        await _repository.SaveAsync();
    }

    public async Task AdicionarAssistenciaAsync(Guid confrontoId, Guid jogadorId)
    {
        var confronto = await _repository.GetByIdAsync(confrontoId);
        if (confronto is null) return;

        var assistencia = new Assistencia
        {
            Id = Guid.NewGuid(),
            ConfrontoId = confrontoId,
            JogadorId = jogadorId,
            Minuto = 0
        };

        await _repository.AddAssistenciaAsync(assistencia);
        await _repository.SaveAsync();
    }

    public async Task AtualizarPlacarAsync(Guid confrontoId, int golsA, int golsB)
    {
        var confronto = await _repository.GetByIdAsync(confrontoId);
        if (confronto is null) return;

        confronto.GolsTimeA = golsA;
        confronto.GolsTimeB = golsB;
        await _repository.SaveAsync();
    }
}
