namespace FutOrganizerWeb.Domain.Entities
{
    public class Partida
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DataHora { get; set; } = DateTime.Now;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Local { get; set; }

        public Guid? EventoId { get; set; }
        public Evento? Evento { get; set; }

        public List<Sorteio> Sorteios { get; set; } = new();

        public Guid? UsuarioCriadorId { get; set; }
        public Usuario? UsuarioCriador { get; set; }
        public string CodigoLobby { get; set; } = Guid.NewGuid().ToString("N")[..6];
        public List<JogadorLobby> JogadoresLobby { get; set; } = new();
        public List<MensagemChat> MensagensChat { get; set; } = new();

    }
}
