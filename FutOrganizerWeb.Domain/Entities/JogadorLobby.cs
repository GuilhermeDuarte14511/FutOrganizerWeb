using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Entities
{
    public class JogadorLobby
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;

        public Guid PartidaId { get; set; }
        public Partida? Partida { get; set; }

        public DateTime DataEntrada { get; set; } = DateTime.Now;
    }

}
