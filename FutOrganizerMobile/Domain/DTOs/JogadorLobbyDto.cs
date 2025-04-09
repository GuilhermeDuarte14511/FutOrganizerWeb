using FutOrganizerWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class JogadorLobby
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; }
        public Guid? UsuarioId { get; set; }

        public Guid PartidaId { get; set; }
        public Partida? Partida { get; set; }

        public DateTime DataEntrada { get; set; } = DateTime.Now;
        public DateTime UltimaAtividade { get; set; } = DateTime.Now;
    }


}
