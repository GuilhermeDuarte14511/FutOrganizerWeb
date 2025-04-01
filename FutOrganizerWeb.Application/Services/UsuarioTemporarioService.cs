using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Services
{
    public class UsuarioTemporarioService : IUsuarioTemporarioService
    {
        private readonly IUsuarioTemporarioRepository _repository;

        public UsuarioTemporarioService(IUsuarioTemporarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<UsuarioTemporario?> ObterPorIdAsync(Guid id)
        {
            return await _repository.ObterPorIdAsync(id);
        }

        public async Task<UsuarioTemporario> ObterOuCriarPorNomeAsync(string nome)
        {
            var existente = await _repository.ObterPorNomeAsync(nome);
            if (existente != null)
                return existente;

            var novo = new UsuarioTemporario
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                DataCriacao = DateTime.UtcNow
            };

            return await _repository.CriarAsync(novo);
        }
    }
}
