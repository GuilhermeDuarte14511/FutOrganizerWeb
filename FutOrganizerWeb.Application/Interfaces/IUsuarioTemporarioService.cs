using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IUsuarioTemporarioService
    {
        Task<UsuarioTemporario> ObterOuCriarPorNomeAsync(string nome);
        Task<UsuarioTemporario?> ObterPorIdAsync(Guid id);
    }
}
