namespace FutOrganizerMobile.Domain.DTOs
{
    public class TimeRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#007bff";
        public List<string> Jogadores { get; set; } = new();
        public string? Goleiro { get; set; }
    }
}