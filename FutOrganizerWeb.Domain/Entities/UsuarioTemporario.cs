using System;

namespace FutOrganizerWeb.Domain.Entities
{
    public class UsuarioTemporario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public List<Partida> Partidas { get; set; } = new();
    }
}
