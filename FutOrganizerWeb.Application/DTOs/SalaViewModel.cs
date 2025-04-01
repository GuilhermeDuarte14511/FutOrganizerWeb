namespace FutOrganizerWeb.Application.DTOs
{
    public class SalaViewModel
    {
        public Guid PartidaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Local { get; set; } = string.Empty;
        public bool Finalizada { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
