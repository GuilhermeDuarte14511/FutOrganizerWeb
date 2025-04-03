namespace FutOrganizerWeb.Application.DTOs
{
    public class JogadorDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime UltimaAtividade { get; set; }
    }
}
