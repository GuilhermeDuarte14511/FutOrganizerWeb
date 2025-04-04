using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class LoginResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public UsuarioLogadoDto Data { get; set; }
    }

}
