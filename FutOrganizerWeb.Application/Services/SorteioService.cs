using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces;
using FutOrganizerWeb.Domain.Interfaces_Repositories;

namespace FutOrganizerWeb.Application.Services
{
    public class SorteioService : ISorteioService
    {
        private readonly ISorteioRepository _repository;

        public SorteioService(ISorteioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CriarSorteioAsync(SorteioRequest request)
        {
            var partida = new Partida
            {
                UsuarioCriadorId = request.UsuarioCriadorId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Local = request.Local,
                DataHora = DateTime.Now
            };

            var sorteio = new Sorteio
            {
                Nome = request.NomeSorteio,
                Data = DateTime.Now
            };

            foreach (var timeReq in request.Times)
            {
                var time = new Time
                {
                    Nome = timeReq.Nome,
                    CorHex = timeReq.CorHex,
                    Jogadores = timeReq.Jogadores.Select(j => new Jogador { Nome = j }).ToList(),
                    Goleiro = string.IsNullOrWhiteSpace(timeReq.Goleiro) ? null : new Goleiro { Nome = timeReq.Goleiro }
                };

                sorteio.Times.Add(time);
            }

            partida.Sorteios.Add(sorteio);

            return await _repository.CriarPartidaComSorteioAsync(partida);
        }

        public async Task<Guid> ResortearAsync(Guid sorteioId, List<TimeRequest> novosTimes)
        {
            var sorteioAntigo = await _repository.ObterSorteioComTimesAsync(sorteioId)
                ?? throw new Exception("Sorteio não encontrado.");

            var partida = sorteioAntigo.Partida;

            if (partida == null)
                throw new Exception("Partida associada ao sorteio não encontrada.");

            var usuarioId = partida.UsuarioCriadorId;
            var latitude = partida.Latitude;
            var longitude = partida.Longitude;
            var local = partida.Local;

            // Remove tudo: jogadores, goleiros, times, sorteio e partida
            await _repository.RemoverPartidaCompletaAsync(partida.Id);

            // Cria nova estrutura com base nos dados anteriores
            var novaPartida = new Partida
            {
                UsuarioCriadorId = usuarioId,
                Latitude = latitude,
                Longitude = longitude,
                Local = local,
                DataHora = DateTime.UtcNow,
                Sorteios = new List<Sorteio>()
            };

            var novoSorteio = new Sorteio
            {
                Nome = $"Sorteio {DateTime.Now:HHmmss}",
                Data = DateTime.UtcNow,
                Times = novosTimes.Select(t => new Time
                {
                    Nome = t.Nome,
                    CorHex = t.CorHex,
                    Jogadores = t.Jogadores.Select(j => new Jogador { Nome = j }).ToList(),
                    Goleiro = string.IsNullOrWhiteSpace(t.Goleiro) ? null : new Goleiro { Nome = t.Goleiro }
                }).ToList()
            };

            novaPartida.Sorteios.Add(novoSorteio);
            return await _repository.CriarPartidaComSorteioAsync(novaPartida);
        }




    }
}
