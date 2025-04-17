using FutOrganizerWeb.Domain.Enums;

namespace FutOrganizerWeb.Domain.Entities
{
    public class Evento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;

        public TipoEvento Tipo { get; set; }

        public decimal? ValorInscricao { get; set; }
        public string? Observacoes { get; set; }

        public List<Partida> Partidas { get; set; } = new();
        public List<EstatisticaJogador> Estatisticas { get; set; } = new();

        public Guid? UsuarioCriadorId { get; set; }
        public Usuario? UsuarioCriador { get; set; }
    }

}
