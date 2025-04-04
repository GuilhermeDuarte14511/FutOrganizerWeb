﻿using System.Net.Http.Json;
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
    }
}
