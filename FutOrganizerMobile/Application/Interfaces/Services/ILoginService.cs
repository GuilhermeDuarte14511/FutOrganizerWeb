using FutOrganizerMobile.Domain.DTOs;

namespace FutOrganizerMobile.Application.Interfaces.Services
{
    public interface ILoginService
    {
        Task<UsuarioLogadoDto?> LogarAsync(string email, string senha);
    }
}
