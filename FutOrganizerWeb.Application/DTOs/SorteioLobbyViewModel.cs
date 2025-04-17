using System;
using System.Collections.Generic;

namespace FutOrganizerWeb.Application.DTOs
{
    public class SorteioLobbyViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public List<string> Jogadores { get; set; } = new();
        public string NomeDoJogador { get; set; } = string.Empty;
        public Guid JogadorId { get; set; } // agora é Guid
        public Guid? UsuarioAutenticadoId { get; set; } // novo campo para saber se é autenticado
        public Guid? EventoId { get; set; } // novo campo para o evento
    }
}
