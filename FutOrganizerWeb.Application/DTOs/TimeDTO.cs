namespace FutOrganizerWeb.Application.DTOs
{
    public class TimeDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#000000";
        public List<JogadorDTO> Jogadores { get; set; } = new();
        public string? Goleiro { get; set; }
    }
}
