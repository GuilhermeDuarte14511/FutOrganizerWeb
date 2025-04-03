using FutOrganizerWeb.Domain.Entities;

public class JogadorLobby
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;

    public Guid PartidaId { get; set; }
    public Partida? Partida { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.Now;
    public DateTime UltimaAtividade { get; set; } = DateTime.Now;
}
