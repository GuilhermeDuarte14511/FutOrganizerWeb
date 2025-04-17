using System;
using System.Collections.Generic;

namespace FutOrganizerWeb.Application.DTOs
{
    public class ConfrontoDTO
    {
        public Guid Id { get; set; }

        public string TimeA { get; set; } = string.Empty;
        public string TimeB { get; set; } = string.Empty;

        public int GolsA { get; set; }
        public int GolsB { get; set; }

        public List<JogadorDTO> JogadoresTimeA { get; set; } = new();
        public List<JogadorDTO> JogadoresTimeB { get; set; } = new();
    }
}
