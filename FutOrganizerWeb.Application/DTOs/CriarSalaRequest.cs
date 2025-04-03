using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class CriarSalaRequest
    {
        public Guid UsuarioId { get; set; }
        public string Local { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

}
