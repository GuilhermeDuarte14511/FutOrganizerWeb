using System;

namespace FutOrganizerMobile.Domain.DTOs
{
    public class JogadorDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; }
        public Guid? UsuarioId { get; set; } = new Guid();
        public Guid? UsuarioAutenticadoId { get; set; }

        public Guid? JogadorLobbyId { get; set; }

        public DateTime UltimaAtividade { get; set; }
    }
}
