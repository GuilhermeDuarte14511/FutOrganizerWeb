using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IUsuarioTemporarioRepository
    {
        Task<UsuarioTemporario?> ObterPorIdAsync(Guid id);
        Task<UsuarioTemporario?> ObterPorNomeAsync(string nome);
        Task<UsuarioTemporario> CriarAsync(UsuarioTemporario usuario);
    }
}
