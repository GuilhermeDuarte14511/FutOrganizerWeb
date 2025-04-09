namespace FutOrganizerWeb.Application.DTOs
{
    public class EntrarLobbyRequest
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid? UsuarioId { get; set; }
    }
}
