using System;
using System.Collections.Generic;

namespace FutOrganizerWeb.Application.DTOs
{
    public class TimeConfrontoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cor { get; set; } = "#000000";
        public List<JogadorDTO> Jogadores { get; set; } = new();
    }
}
