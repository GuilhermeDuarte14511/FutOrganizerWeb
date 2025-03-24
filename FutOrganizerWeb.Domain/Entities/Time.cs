namespace FutOrganizerWeb.Domain.Entities
{
    public class Time
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#000000";

        public List<Jogador> Jogadores { get; set; } = new();
        public Goleiro? Goleiro { get; set; }

        public bool EstaIncompleto(int jogadoresPorTime)
        {
            return Jogadores.Count < jogadoresPorTime;
        }
    }
}
