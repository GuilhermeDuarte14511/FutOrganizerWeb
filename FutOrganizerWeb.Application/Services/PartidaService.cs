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

        public async Task<List<Partida>> ObterPartidasPorUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina)
        {
            return await _repository.ObterPartidasPorUsuarioAsync(usuarioId, pagina, tamanhoPagina);
        }


        public async Task<List<Partida>> ObterPartidasPaginadasPorUsuarioAsync(Guid usuarioId, int page, int pageSize)
        {
            return await _repository.ObterPartidasPaginadasPorUsuarioAsync(usuarioId, page, pageSize);
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

        public async Task<Partida?> ObterPorCodigoAsync(string codigo)
        {
            return await _repository.ObterPorCodigoAsync(codigo);
        }

        public async Task<JogadorDTO> AdicionarJogadorAoLobbyAsync(string codigo, string nomeJogador, string? email = null, Guid? usuarioAutenticadoId = null)
        {
            var partida = await _repository.ObterPorCodigoAsync(codigo);
            if (partida == null)
                throw new Exception("Partida não encontrada");

            JogadorLobby? jogadorExistente = null;

            if (usuarioAutenticadoId.HasValue)
            {
                jogadorExistente = partida.JogadoresLobby
                    .FirstOrDefault(j => j.UsuarioAutenticadoId == usuarioAutenticadoId);
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                jogadorExistente = partida.JogadoresLobby
                    .FirstOrDefault(j => j.Email != null && j.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                jogadorExistente = partida.JogadoresLobby
                    .FirstOrDefault(j => j.Nome.Equals(nomeJogador, StringComparison.OrdinalIgnoreCase));
            }

            if (jogadorExistente != null)
            {
                return new JogadorDTO
                {
                    Id = jogadorExistente.Id,
                    Nome = jogadorExistente.Nome,
                    Email = jogadorExistente.Email,
                    UsuarioId = jogadorExistente.UsuarioAutenticadoId,
                    UltimaAtividade = jogadorExistente.UltimaAtividade
                };
            }

            var novoJogador = new JogadorLobby
            {
                Nome = nomeJogador,
                Email = email,
                UsuarioAutenticadoId = usuarioAutenticadoId,
                PartidaId = partida.Id,
                DataEntrada = DateTime.Now,
                UltimaAtividade = DateTime.Now
            };

            await _repository.AdicionarJogadorAsync(novoJogador);

            return new JogadorDTO
            {
                Id = novoJogador.Id,
                Nome = novoJogador.Nome,
                Email = novoJogador.Email,
                UsuarioId = novoJogador.UsuarioAutenticadoId,
                UltimaAtividade = novoJogador.UltimaAtividade
            };
        }




        public async Task RemoverJogadorAsync(string codigo, Guid jogadorId)
        {
            await _repository.RemoverJogadorAsync(codigo, jogadorId);
        }

        public async Task CriarPartidaAsync(Partida partida)
        {
            await _repository.CriarPartidaAsync(partida);
        }

        public async Task<SorteioDTO?> ObterSorteioDaPartidaAsync(string codigo)
        {
            var partida = await _repository.ObterPorCodigoAsync(codigo);

            if (partida == null)
                return null;

            var sorteio = partida.Sorteios
                .OrderByDescending(s => s.Data)
                .FirstOrDefault();

            if (sorteio == null)
                return null;

            return new SorteioDTO
            {
                Id = sorteio.Id,
                Nome = sorteio.Nome,
                Data = sorteio.Data,
                Times = sorteio.Times.Select(t => new TimeDTO
                {
                    Nome = t.Nome,
                    CorHex = t.CorHex,
                    Jogadores = t.Jogadores.Select(j => new JogadorDTO
                    {
                        Id = j.Id,
                        Nome = j.Nome
                    }).ToList(),
                    Goleiro = t.Goleiro?.Nome
                }).ToList()
            };
        }

        public async Task AtualizarUltimaAtividadeAsync(string codigo, string nomeJogador)
        {
            var partida = await _repository.ObterPorCodigoAsync(codigo);
            if (partida == null) return;

            var jogador = partida.JogadoresLobby.FirstOrDefault(j => j.Nome == nomeJogador);
            if (jogador != null)
            {
                jogador.UltimaAtividade = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

                await _repository.SalvarAsync();
            }
        }

        public Task<List<Partida>> ObterSalasOndeParticipeiAsync(Guid usuarioId)
        {
            return _repository.ObterSalasOndeUsuarioParticipouAsync(usuarioId);
        }

        public async Task<JogadorDTO> AdicionarJogadorAoLobbyAsync(JogadorLobby jogador)
        {
            var partida = await _repository.ObterPorIdAsync(jogador.PartidaId);
            if (partida == null)
                throw new Exception("Partida não encontrada");

            var jogadorExistente = partida.JogadoresLobby.FirstOrDefault(j =>
                (!string.IsNullOrWhiteSpace(j.Email) && j.Email == jogador.Email) ||
                (!string.IsNullOrWhiteSpace(j.Nome) && j.Nome == jogador.Nome && j.UsuarioAutenticadoId == jogador.UsuarioAutenticadoId));

            if (jogadorExistente != null)
            {
                return new JogadorDTO
                {
                    Id = jogadorExistente.Id,
                    Nome = jogadorExistente.Nome,
                    Email = jogadorExistente.Email,
                    UsuarioId = jogadorExistente.UsuarioAutenticadoId,
                    UltimaAtividade = jogadorExistente.UltimaAtividade
                };
            }

            await _repository.AdicionarJogadorAsync(jogador);

            return new JogadorDTO
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                UsuarioId = jogador.UsuarioAutenticadoId,
                UltimaAtividade = jogador.UltimaAtividade
            };
        }


    }
}