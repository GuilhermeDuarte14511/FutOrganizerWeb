using FutOrganizerWeb.Domain.Entities;

public class EstatisticaJogador
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NomeJogador { get; set; } = string.Empty;

    public Guid? UsuarioAutenticadoId { get; set; } // se for cadastrado
    public Guid? JogadorLobbyId { get; set; } // se for convidado
    public string? Email { get; set; } // fallback

    public int Gols { get; set; }
    public int Assistencias { get; set; }
    public int Defesas { get; set; }

    public Guid EventoId { get; set; }
    public Evento? Evento { get; set; }
}

