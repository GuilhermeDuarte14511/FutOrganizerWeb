using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class JogadorDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid? UsuarioId { get; set; }
        public DateTime UltimaAtividade { get; set; }
    }

}
