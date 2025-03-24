namespace FutOrganizerWeb.Domain.Entities
{
    public class Sorteio
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;

        public Guid PartidaId { get; set; }
        public Partida? Partida { get; set; }

        public List<Time> Times { get; set; } = new();
    }
}
