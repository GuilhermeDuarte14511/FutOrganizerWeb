using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarBoasVindasAsync(string nome, string email, string loginUrl);
        Task EnviarRecuperacaoSenhaAsync(string nome, string email, string linkRedefinicao);

    }
}
