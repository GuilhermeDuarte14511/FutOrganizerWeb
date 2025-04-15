using FutOrganizerWeb.Domain.Entities;

public class JogadorLobby
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;

    // Novo relacionamento com o usuário autenticado
    public Guid? UsuarioAutenticadoId { get; set; }
    public Usuario? UsuarioAutenticado { get; set; }

    // E-mail opcional (para convidados, por exemplo)
    public string? Email { get; set; }

    // Relacionamento com a Partida
    public Guid PartidaId { get; set; }
    public Partida? Partida { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.Now;
    public DateTime UltimaAtividade { get; set; } = DateTime.Now;
}
