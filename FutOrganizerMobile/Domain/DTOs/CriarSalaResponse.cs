using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class CriarSalaResponse
    {
        public bool Sucesso { get; set; }
        public string CodigoLobby { get; set; } = string.Empty;
    }

}
