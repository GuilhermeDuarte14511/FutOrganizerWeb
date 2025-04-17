using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repository;

        public EventoService(IEventoRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventoDTO?> ObterPorIdAsync(Guid id)
        {
            var evento = await _repository.ObterPorIdAsync(id);
            if (evento == null) return null;

            var partida = evento.Partidas?.FirstOrDefault();

            return new EventoDTO
            {
                Id = evento.Id,
                Nome = evento.Nome,
                Data = evento.Data,
                Tipo = evento.Tipo,
                ValorInscricao = evento.ValorInscricao,
                Observacoes = evento.Observacoes,
                UsuarioCriadorId = evento.UsuarioCriadorId,
                Partida = partida != null
                    ? new EventoDTO.PartidaInfoDTO
                    {
                        PartidaId = partida.Id,
                        CodigoLobby = partida.CodigoLobby,
                        DataCriacao = partida.DataHora
                    }
                    : null
            };
        }


        public async Task<List<EventoDTO>> ListarEventosDoUsuarioAsync(Guid usuarioId)
        {
            var eventos = await _repository.ObterTodosDoUsuarioAsync(usuarioId);
            return eventos.Select(e => new EventoDTO
            {
                Id = e.Id,
                Nome = e.Nome,
                Data = e.Data,
                UsuarioCriadorId = e.UsuarioCriadorId
            }).ToList();
        }

        public async Task<Guid> CriarEventoAsync(EventoDTO dto)
        {
            var evento = new Evento
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Data = dto.Data,
                Tipo = dto.Tipo,
                ValorInscricao = dto.ValorInscricao,
                Observacoes = dto.Observacoes,
                UsuarioCriadorId = dto.UsuarioCriadorId
            };

            await _repository.CriarAsync(evento);
            return evento.Id;
        }

        public async Task<bool> EditarEventoAsync(EventoDTO dto)
        {
            var evento = await _repository.ObterPorIdAsync(dto.Id);
            if (evento == null || evento.UsuarioCriadorId != dto.UsuarioCriadorId)
                return false;

            evento.Nome = dto.Nome;
            await _repository.AtualizarAsync(evento);
            return true;
        }

        public async Task<bool> RemoverEventoAsync(Guid id, Guid usuarioId)
        {
            var evento = await _repository.ObterPorIdAsync(id);
            if (evento == null || evento.UsuarioCriadorId != usuarioId)
                return false;

            await _repository.ExcluirAsync(id);
            return true;
        }
    }
}
