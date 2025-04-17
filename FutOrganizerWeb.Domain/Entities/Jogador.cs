using System;

namespace FutOrganizerWeb.Domain.Entities
{
    public class Jogador
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; }

        public Guid? UsuarioAutenticadoId { get; set; }

        public Guid? JogadorLobbyId { get; set; }

        public DateTime UltimaAtividade { get; set; } = DateTime.UtcNow;
    }
}
