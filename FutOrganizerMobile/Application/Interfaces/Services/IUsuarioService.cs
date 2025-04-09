using FutOrganizerMobile.Domain.DTOs;

namespace FutOrganizerMobile.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<ApiResponse> CadastrarUsuarioAsync(CadastrarUsuarioRequest request);
    }
}
