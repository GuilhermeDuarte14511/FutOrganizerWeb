using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class HistoricoPartidaDTO
    {
        public Guid PartidaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
