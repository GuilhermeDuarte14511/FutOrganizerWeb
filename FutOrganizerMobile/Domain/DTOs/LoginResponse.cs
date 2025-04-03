using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class LoginResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
        public Guid idUsuario { get; set; }
    }
}
