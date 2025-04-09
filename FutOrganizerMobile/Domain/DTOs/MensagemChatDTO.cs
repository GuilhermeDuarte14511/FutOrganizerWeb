using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class MensagemChatDTO
    {
        public string NomeUsuario { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
    }
}
