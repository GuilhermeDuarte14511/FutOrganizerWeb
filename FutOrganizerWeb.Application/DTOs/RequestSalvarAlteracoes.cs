using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class RequestSalvarAlteracoes
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? SenhaAtual { get; set; }
        public string? NovaSenha { get; set; }
    }
}
