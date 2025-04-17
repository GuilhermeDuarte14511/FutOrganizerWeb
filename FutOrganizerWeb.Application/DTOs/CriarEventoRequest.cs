using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class CriarEventoRequest
    {
        [Required(ErrorMessage = "O nome do evento é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public Guid UsuarioCriadorId { get; set; }
    }
}
