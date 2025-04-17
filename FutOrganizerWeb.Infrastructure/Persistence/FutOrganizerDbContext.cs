using FutOrganizerWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Persistence
{
    public class FutOrganizerDbContext : DbContext
    {
        public FutOrganizerDbContext(DbContextOptions<FutOrganizerDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Sorteio> Sorteios { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Jogador> Jogadores { get; set; }
        public DbSet<Goleiro> Goleiros { get; set; }
        public DbSet<EstatisticaJogador> Estatisticas { get; set; }
        public DbSet<ConfiguracaoPelada> ConfiguracoesPelada { get; set; }
        public DbSet<JogadorLobby> JogadoresLobby { get; set; }
        public DbSet<UsuarioTemporario> UsuariosTemporarios { get; set; }
        public DbSet<MensagemChat> MensagensChat { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Confronto> Confrontos { get; set; }
        public DbSet<Gol> Gols { get; set; }
        public DbSet<Assistencia> Assistencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chaves primárias
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Evento>().HasKey(e => e.Id);
            modelBuilder.Entity<Partida>().HasKey(p => p.Id);
            modelBuilder.Entity<Sorteio>().HasKey(s => s.Id);
            modelBuilder.Entity<Time>().HasKey(t => t.Id);
            modelBuilder.Entity<Jogador>().HasKey(j => j.Id);
            modelBuilder.Entity<Goleiro>().HasKey(g => g.Id);
            modelBuilder.Entity<EstatisticaJogador>().HasKey(e => e.Id);
            modelBuilder.Entity<JogadorLobby>().HasKey(j => j.Id);
            modelBuilder.Entity<UsuarioTemporario>().HasKey(u => u.Id);
            modelBuilder.Entity<MensagemChat>().HasKey(m => m.Id);
            modelBuilder.Entity<EmailTemplate>().HasKey(e => e.Id);
            modelBuilder.Entity<Confronto>().HasKey(c => c.Id);
            modelBuilder.Entity<Gol>().HasKey(g => g.Id);
            modelBuilder.Entity<Assistencia>().HasKey(a => a.Id);

            // Relacionamentos principais

            // Evento
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.UsuarioCriador)
                .WithMany(u => u.EventosCriados)
                .HasForeignKey(e => e.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Partidas)
                .WithOne(p => p.Evento)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Estatisticas)
                .WithOne(est => est.Evento)
                .HasForeignKey(est => est.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Partida
            modelBuilder.Entity<Partida>()
                .HasOne(p => p.UsuarioCriador)
                .WithMany(u => u.PartidasCriadas)
                .HasForeignKey(p => p.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasMany(p => p.Sorteios)
                .WithOne(s => s.Partida)
                .HasForeignKey(s => s.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JogadorLobby>()
                .HasOne(j => j.Partida)
                .WithMany(p => p.JogadoresLobby)
                .HasForeignKey(j => j.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JogadorLobby>()
                .HasOne(j => j.UsuarioAutenticado)
                .WithMany()
                .HasForeignKey(j => j.UsuarioAutenticadoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MensagemChat>()
                .HasOne(m => m.Partida)
                .WithMany(p => p.MensagensChat)
                .HasForeignKey(m => m.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sorteios → Confrontos
            modelBuilder.Entity<Confronto>()
                .HasOne(c => c.Sorteio)
                .WithMany(s => s.Confrontos)
                .HasForeignKey(c => c.SorteioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Confrontos
            modelBuilder.Entity<Confronto>()
                .HasOne(c => c.TimeA)
                .WithMany()
                .HasForeignKey(c => c.TimeAId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Confronto>()
                .HasOne(c => c.TimeB)
                .WithMany()
                .HasForeignKey(c => c.TimeBId)
                .OnDelete(DeleteBehavior.Restrict);

            // Gols
            modelBuilder.Entity<Gol>()
                .HasOne(g => g.Confronto)
                .WithMany(c => c.Gols)
                .HasForeignKey(g => g.ConfrontoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gol>()
                .HasOne(g => g.Jogador)
                .WithMany()
                .HasForeignKey(g => g.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assistências
            modelBuilder.Entity<Assistencia>()
                .HasOne(a => a.Confronto)
                .WithMany(c => c.Assistencias)
                .HasForeignKey(a => a.ConfrontoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assistencia>()
                .HasOne(a => a.Jogador)
                .WithMany()
                .HasForeignKey(a => a.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConfiguracaoPelada>().HasNoKey();
        }
    }
}
