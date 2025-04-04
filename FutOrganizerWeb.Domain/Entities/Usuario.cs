﻿namespace FutOrganizerWeb.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        // Relacionamentos
        public List<Partida> PartidasCriadas { get; set; } = new();
        public List<Evento> EventosCriados { get; set; } = new();
    }
}
