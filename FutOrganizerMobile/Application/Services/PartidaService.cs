using System.Net.Http.Json;
using System.Text.Json;
using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Utils;

namespace FutOrganizerMobile.Application.Services
{
    public class PartidaService : IPartidaService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl = "https://futorganizerapi-bxc4d0egepcuh3gx.brazilsouth-01.azurewebsites.net";

        public PartidaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<HistoricoPartidaDto>> ObterHistoricoAsync(Guid usuarioId, int pagina = 1, int quantidade = 10)
        {
            var url = $"{_baseUrl}/api/Partida/historico?usuarioId={usuarioId}&pagina={pagina}&quantidade={quantidade}";

            try
            {
                return await _http.GetFromJsonAsync<List<HistoricoPartidaDto>>(url) ?? new List<HistoricoPartidaDto>();
            }
            catch
            {
                return new List<HistoricoPartidaDto>();
            }
        }

        public async Task<PartidaLobbyDto?> ObterPorCodigoAsync(string codigo)
        {
            var url = $"{_baseUrl}/api/Partida/por-codigo?codigo={codigo}";

            try
            {
                return await _http.GetFromJsonAsync<PartidaLobbyDto>(url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> CriarSalaAsync(CriarSalaRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/Sorteio/criar-sala", request);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<CriarSalaResponse>();
                    return resultado?.CodigoLobby;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CriarSorteioAsync(SorteioRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/Sorteio/criar", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<SalaViewModel>> ObterSalasAsync(Guid usuarioId, int pagina = 1, int tamanhoPagina = 10)
        {
            var url = $"{_baseUrl}/api/Sorteio/salas?usuarioId={usuarioId}&pagina={pagina}&tamanhoPagina={tamanhoPagina}";

            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new List<SalaViewModel>();

                var content = await response.Content.ReadAsStringAsync();

                var salas = JsonSerializer.Deserialize<List<SalaViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<SalaViewModel>();

                foreach (var sala in salas)
                {
                    if (sala.Local == "Não informado")
                    {
                        sala.Local = await AppHelper.ObterEnderecoPorCoordenadasAsync(sala.Latitude, sala.Longitude);
                    }
                }

                return salas;
            }
            catch
            {
                return new List<SalaViewModel>();
            }
        }

        public async Task<JogadorDTO?> EntrarNaSalaComoLogadoAsync(string codigo, JogadorDTO jogador)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/entrar";
                var request = new
                {
                    Codigo = codigo,
                    Nome = jogador.Nome,
                    Email = jogador.Email,
                    UsuarioId = jogador.UsuarioId
                };

                var response = await _http.PostAsJsonAsync(url, request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JogadorDTO>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }


        public async Task<List<MensagemChatDTO>> ObterMensagensChatAsync(string codigo)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/mensagens";
                return await _http.GetFromJsonAsync<List<MensagemChatDTO>>(url) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<MensagemChatDTO?> EnviarMensagemChatAsync(string codigo, string nome, string conteudo)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/mensagem";

                var request = new
                {
                    NomeUsuario = nome,
                    Conteudo = conteudo
                };

                var response = await _http.PostAsJsonAsync(url, request);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<MensagemChatDTO>();
            }
            catch
            {
                return null;
            }
        }

        public async Task AtualizarAtividadeAsync(string codigo, string nomeJogador)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/atualizar-atividade";

                var request = new
                {
                    NomeJogador = nomeJogador
                };

                await _http.PostAsJsonAsync(url, request);
            }
            catch
            {
                // Silenciosamente ignorar
            }
        }

        public async Task<List<JogadorDTO>> ObterJogadoresAsync(string codigo)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/jogadores";
                return await _http.GetFromJsonAsync<List<JogadorDTO>>(url) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<List<string>> ObterUsuariosOnlineAsync(string codigo)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/online";
                return await _http.GetFromJsonAsync<List<string>>(url) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<(bool Sucesso, SorteioDTO? Sorteio)> VerificarSorteioAsync(string codigo)
        {
            try
            {
                var url = $"{_baseUrl}/api/lobby/{codigo}/verificar-sorteio";
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return (false, null);

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                var sorteioRealizado = doc.RootElement.GetProperty("sorteioRealizado").GetBoolean();
                if (!sorteioRealizado)
                    return (false, null);

                var sorteioJson = doc.RootElement.GetProperty("sorteio").GetRawText();
                var sorteio = JsonSerializer.Deserialize<SorteioDTO>(sorteioJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return (true, sorteio);
            }
            catch
            {
                return (false, null);
            }
        }


    }
}
