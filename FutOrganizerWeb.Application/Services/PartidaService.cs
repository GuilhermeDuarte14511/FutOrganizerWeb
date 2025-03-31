using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;

namespace FutOrganizerWeb.Application.Services
{
    public class PartidaService : IPartidaService
    {
        private readonly IPartidaRepository _repository;

        public PartidaService(IPartidaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId)
        {
            return await _repository.ObterPartidasPorUsuarioAsync(usuarioId);
        }

        public async Task<DetalhesPartidaDTO> ObterDetalhesDaPartidaAsync(Guid partidaId)
        {
            var partida = await _repository.ObterPartidaComSorteioAsync(partidaId)
                ?? throw new Exception("Partida não encontrada.");

            var sorteio = partida.Sorteios
                .OrderByDescending(s => s.Data)
                .FirstOrDefault();

            return new DetalhesPartidaDTO
            {
                PartidaId = partida.Id,
                DataHora = partida.DataHora,
                Local = partida.Local,
                Latitude = partida.Latitude ?? 0,
                Longitude = partida.Longitude ?? 0,
                NomeSorteio = sorteio?.Nome ?? "Sorteio",
                Times = sorteio?.Times
                    .OrderBy(t =>
                    {
                        var numeroStr = new string(t.Nome.Where(char.IsDigit).ToArray());
                        return int.TryParse(numeroStr, out var numero) ? numero : int.MaxValue;
                    })
                    .Select(t => new TimeDetalhadoDTO
                    {
                        Nome = t.Nome,
                        CorHex = t.CorHex,
                        Goleiro = t.Goleiro != null ? new GoleiroDTO { Nome = t.Goleiro.Nome } : null,
                        Jogadores = t.Jogadores.Select(j => new JogadorDTO { Nome = j.Nome }).ToList()
                    }).ToList() ?? new()
            };
        }


    }
}
