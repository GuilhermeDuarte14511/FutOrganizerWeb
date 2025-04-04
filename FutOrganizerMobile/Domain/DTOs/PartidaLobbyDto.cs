using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class PartidaLobbyDto
    {
        public string CodigoLobby { get; set; } = string.Empty;
        public string? Local { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime DataHora { get; set; }

        public Guid UsuarioCriadorId { get; set; }
        public string NomeCriador { get; set; } = string.Empty;

        public List<JogadorLobbyDto> JogadoresLobby { get; set; } = new();
    }


}
