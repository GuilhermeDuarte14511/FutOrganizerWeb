using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class TimeDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#000000";
        public List<string> Jogadores { get; set; } = new();
        public string? Goleiro { get; set; }
    }
}
