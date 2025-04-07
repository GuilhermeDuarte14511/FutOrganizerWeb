using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Utils
{
    public static class AppHelper
    {
        public static string GenerateEmbedMapUrl(string location)
        {
            if (string.IsNullOrEmpty(location))
                return "https://upload.wikimedia.org/wikipedia/commons/8/88/Map_marker.png";

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
            catch
            {
                // Ignore e usa fallback abaixo
            }

            return "https://upload.wikimedia.org/wikipedia/commons/8/88/Map_marker.png";
        }

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
            catch
            {
                // Ignore e usa fallback abaixo
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
            catch
            {
                // Ignore e usa fallback abaixo
            }

            return "https://www.openstreetmap.org/";
        }

            public static async Task<string> ObterEnderecoPorCoordenadasAsync(double latitude, double longitude)
            {
                try
                {
                    var latStr = latitude.ToString(CultureInfo.InvariantCulture);
                    var lonStr = longitude.ToString(CultureInfo.InvariantCulture);

                    using var httpClient = new HttpClient();
                    string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latStr}&lon={lonStr}&zoom=18&addressdetails=1";

                    httpClient.DefaultRequestHeaders.Add("User-Agent", "FutOrganizerMobile/1.0");

                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return "Endereço não encontrado";

                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(content);

                    if (!json.RootElement.TryGetProperty("address", out var address))
                        return "Endereço não disponível";

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
