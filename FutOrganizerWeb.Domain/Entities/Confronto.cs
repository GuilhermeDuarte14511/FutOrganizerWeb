using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Entities
{
    public class Confronto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SorteioId { get; set; }
        public Sorteio? Sorteio { get; set; }

        public Guid TimeAId { get; set; }
        public Time? TimeA { get; set; }

        public Guid TimeBId { get; set; }
        public Time? TimeB { get; set; }

        public int GolsTimeA { get; set; } = 0;
        public int GolsTimeB { get; set; } = 0;

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public ICollection<Gol> Gols { get; set; } = new List<Gol>();
        public ICollection<Assistencia> Assistencias { get; set; } = new List<Assistencia>();
    }

}
