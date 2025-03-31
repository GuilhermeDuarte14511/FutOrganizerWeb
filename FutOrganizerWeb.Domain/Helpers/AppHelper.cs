using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Helpers
{
    public static class AppHelper
    {
        /// <summary>
        /// Gera uma URL do OpenStreetMap para exibição do mapa incorporado (embed).
        /// </summary>
        public static string GenerateEmbedMapUrl(string location)
        {
            if (string.IsNullOrEmpty(location))
                return "https://upload.wikimedia.org/wikipedia/commons/8/88/Map_marker.png"; // Fallback

            try
            {
                var parts = location.Split(',');

                if (parts.Length != 2)
                {
                    location = NormalizeLocation(location);
                    parts = location.Split(',');
                }

                if (parts.Length == 2)
                {
                    string latitude = parts[0].Trim();
                    string longitude = parts[1].Trim();

                    return $"https://www.openstreetmap.org/export/embed.html?bbox={longitude},{latitude},{longitude},{latitude}&layer=mapnik&marker={latitude},{longitude}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar coordenadas: {ex.Message}");
            }

            return "https://upload.wikimedia.org/wikipedia/commons/8/88/Map_marker.png";
        }

        /// <summary>
        /// Normaliza a string de localização garantindo que a latitude e longitude fiquem no formato correto.
        /// </summary>
        public static string NormalizeLocation(string location)
        {
            try
            {
                if (string.IsNullOrEmpty(location))
                    return "0.0,0.0";

                location = location.Trim();
                string[] parts = location.Split(',');

                if (parts.Length >= 4)
                {
                    string latitude = $"{parts[0].Trim()}.{parts[1].Trim()}";
                    string longitude = $"{parts[2].Trim()}.{parts[3].Trim()}";

                    return $"{latitude},{longitude}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao normalizar localização: {ex.Message}");
            }

            return "0.0,0.0";
        }

        public static string GenerateOpenMapUrl(string location)
        {
            if (string.IsNullOrEmpty(location))
                return "https://www.openstreetmap.org/";

            try
            {
                var parts = location.Split(',');

                // Verifica se já está no formato correto
                if (parts.Length != 2)
                {
                    location = NormalizeLocation(location);
                    parts = location.Split(',');
                }

                if (parts.Length == 2)
                {
                    string latitude = parts[0].Trim();
                    string longitude = parts[1].Trim();

                    return $"https://www.openstreetmap.org/?mlat={latitude}&mlon={longitude}#map=15/{latitude}/{longitude}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar coordenadas: {ex.Message}");
            }

            return "https://www.openstreetmap.org/";
        }

        public static async Task<string> ObterEnderecoPorCoordenadasAsync(double latitude, double longitude)
        {
            try
            {
                var latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                using var httpClient = new HttpClient();
                string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latStr}&lon={lonStr}&zoom=18&addressdetails=1";

                httpClient.DefaultRequestHeaders.Add("User-Agent", "FutOrganizerWeb/1.0");

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return "Endereço não encontrado";

                var content = await response.Content.ReadAsStringAsync();
                var json = System.Text.Json.JsonDocument.Parse(content);

                var address = json.RootElement.GetProperty("address");

                string rua = address.TryGetProperty("road", out var roadProp) ? roadProp.GetString() ?? "" : "";
                string cidade = address.TryGetProperty("city", out var cityProp) ? cityProp.GetString() ?? "" : "";
                string estado = address.TryGetProperty("state", out var stateProp) ? stateProp.GetString() ?? "" : "";

                string enderecoFinal = $"{rua}, {cidade} - {estado}".Trim(' ', ',');
                return string.IsNullOrWhiteSpace(enderecoFinal) ? "Endereço não disponível" : enderecoFinal;
            }
            catch (Exception ex)
            {
                return $"Erro ao buscar endereço: {ex.Message}";
            }
        }



    }
}
