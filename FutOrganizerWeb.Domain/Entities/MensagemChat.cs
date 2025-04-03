using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Entities
{
    public class MensagemChat
    {
        public Guid Id { get; set; }
        public Guid PartidaId { get; set; }              
        public string NomeJogador { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public DateTime DataHoraEnvio { get; set; } = DateTime.Now;

        public Partida? Partida { get; set; }
    }

}
