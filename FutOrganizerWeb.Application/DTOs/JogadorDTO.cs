namespace FutOrganizerWeb.Application.DTOs
{
    public class JogadorDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; }
        public Guid? UsuarioId { get; set; }

        public DateTime UltimaAtividade { get; set; }
        public int Gols { get; set; }
        public int Assistencias { get; set; }
    }
}
