using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class TimeModel
    {
        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#007bff";
        public List<string> Jogadores { get; set; } = new();
        public string? Goleiro { get; set; }
    }
}
