using FutOrganizerWeb.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FutOrganizerWeb.Application.Interfaces
{
    public interface IAuthService
    {
        string GerarHash(string senha);
        bool VerificarHash(string senhaDigitada, string hashArmazenado);
        void SalvarUsuarioNaSessao(HttpContext context, Usuario usuario);

    }
}
