// DetalhesPartidaDTO.cs
namespace FutOrganizerWeb.Application.DTOs
{
    public class DetalhesPartidaDTO
    {
        public Guid PartidaId { get; set; }
        public string CodigoLobby { get; set; } = string.Empty;
        public string NomeSorteio { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Local { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<TimeDetalhadoDTO> Times { get; set; } = new();
    }

}
