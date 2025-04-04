using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FutOrganizerMobile.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://futorganizerapi-bxc4d0egepcuh3gx.brazilsouth-01.azurewebsites.net";

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UsuarioLogadoDto?> LogarAsync(string email, string senha)
        {
            var payload = new { email, senha };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/login/logar", content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null && result.Sucesso)
                    return result.Data;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
