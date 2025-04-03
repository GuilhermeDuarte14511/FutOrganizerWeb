using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Application.Interfaces.Services
{
    public interface ILoginService
    {
        Task<(bool sucesso, string mensagem, Guid? usuarioId)> LogarAsync(string email, string senha);
    }
}
