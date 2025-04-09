using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using System.Net.Http.Json;

namespace FutOrganizerMobile.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://futorganizerapi-bxc4d0egepcuh3gx.brazilsouth-01.azurewebsites.net");
        }

        public async Task<ApiResponse> CadastrarUsuarioAsync(CadastrarUsuarioRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Usuario/cadastrar", request);

                if (response.IsSuccessStatusCode)
                {
                    var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return apiResult ?? new ApiResponse { Sucesso = false, Mensagem = "Resposta inesperada da API." };
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    return new ApiResponse
                    {
                        Sucesso = false,
                        Mensagem = $"Erro: {response.StatusCode} - {erro}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao conectar com a API: {ex.Message}"
                };
            }
        }
    }
}
