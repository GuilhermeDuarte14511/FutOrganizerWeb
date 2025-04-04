using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class HistoricoPartidaDto
    {
        public Guid PartidaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string? Local { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string MapaUrl =>
            $"https://www.openstreetmap.org/export/embed.html?bbox={Longitude},{Latitude},{Longitude},{Latitude}&layer=mapnik&marker={Latitude},{Longitude}";
    }

}
