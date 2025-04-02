using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class SorteioRequest
    {
        public string NomeSorteio { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Local { get; set; }
        public Guid UsuarioCriadorId { get; set; }
        public List<TimeRequest> Times { get; set; } = new();
        public string CodigoLobby { get; set; }

    }


}
