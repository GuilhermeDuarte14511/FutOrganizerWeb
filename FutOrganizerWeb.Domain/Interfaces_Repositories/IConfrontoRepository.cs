using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Interfaces_Repositories
{
    public interface IConfrontoRepository
    {
        Task AddAsync(Confronto confronto);
        Task<List<Confronto>> GetBySorteioAsync(Guid sorteioId);
        Task<Confronto?> GetByIdAsync(Guid id);
        Task SaveAsync();

        Task AddGolAsync(Gol gol);
        Task AddAssistenciaAsync(Assistencia assistencia);
    }
}
