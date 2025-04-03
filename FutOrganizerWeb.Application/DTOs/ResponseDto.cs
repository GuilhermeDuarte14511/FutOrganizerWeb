using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class ResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public object Data { get; set; }
    }

}
