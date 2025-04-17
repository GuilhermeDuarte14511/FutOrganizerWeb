using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Entities
{
    public class Assistencia
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ConfrontoId { get; set; }
        public Confronto? Confronto { get; set; }

        public Guid JogadorId { get; set; }
        public Jogador? Jogador { get; set; }

        public int Minuto { get; set; }
    }

}
