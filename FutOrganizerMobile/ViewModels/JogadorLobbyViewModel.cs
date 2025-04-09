using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.ViewModels
{
    public class JogadorLobbyViewModel
    {
        public string Nome { get; set; } = "";
        public string UltimaAtividadeFormatada { get; set; } = "";
        public Color CorStatus { get; set; } = Colors.Red;
    }
}
