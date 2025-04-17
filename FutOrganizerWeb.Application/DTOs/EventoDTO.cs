using FutOrganizerWeb.Domain.Enums;
using System;

namespace FutOrganizerWeb.Application.DTOs
{
    public class EventoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public TipoEvento Tipo { get; set; }
        public decimal? ValorInscricao { get; set; }
        public string? Observacoes { get; set; }
        public Guid? UsuarioCriadorId { get; set; }

        // ✅ Indica se já tem sorteio vinculado
        public bool PossuiSorteio => Partida != null;

        // ✅ Informações da Partida (caso exista)
        public PartidaInfoDTO? Partida { get; set; }

        public class PartidaInfoDTO
        {
            public Guid PartidaId { get; set; }
            public string CodigoLobby { get; set; } = string.Empty;
            public DateTime DataCriacao { get; set; }
        }
    }
}
