// TimeDetalhadoDTO.cs
namespace FutOrganizerWeb.Application.DTOs
{
    public class TimeDetalhadoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#000000";
        public List<JogadorDTO> Jogadores { get; set; } = new();
        public GoleiroDTO? Goleiro { get; set; }
    }
}
