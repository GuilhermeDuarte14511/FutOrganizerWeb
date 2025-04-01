using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class SorteioLobbyViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public List<string> Jogadores { get; set; } = new();
    }

}
