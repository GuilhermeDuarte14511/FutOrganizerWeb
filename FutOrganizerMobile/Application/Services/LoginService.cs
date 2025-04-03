using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using System.Net.Http.Json;

namespace FutOrganizerMobile.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;

        // Definindo o valor manualmente para a BaseUrl
        private readonly string _baseUrl = "http://192.168.15.6:7159"; // Valor manualmente atribuído

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool sucesso, string mensagem, Guid? usuarioId)> LogarAsync(string email, string senha)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("senha", senha)
            });

            try
            {
                // Usando o valor fixo da BaseUrl
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/login/logar", content);
                if (!response.IsSuccessStatusCode)
                    return (false, "Erro de autenticação", null);

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null && result.sucesso)
                    return (true, result.mensagem ?? "", result.idUsuario);

                return (false, result?.mensagem ?? "Falha no login", null);
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}", null);
            }
        }
    }
}
