using FutOrganizerWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Persistence
{
    public class FutOrganizerDbContext : DbContext
    {
        public FutOrganizerDbContext(DbContextOptions<FutOrganizerDbContext> options)
            : base(options)
        {
        }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chaves primárias
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Evento>().HasKey(e => e.Id);
            modelBuilder.Entity<Partida>().HasKey(p => p.Id);
            modelBuilder.Entity<Sorteio>().HasKey(s => s.Id);
            modelBuilder.Entity<Time>().HasKey(t => t.Id);
            modelBuilder.Entity<EstatisticaJogador>().HasKey(e => e.Id);
            modelBuilder.Entity<Jogador>().HasKey(j => j.Id);
            modelBuilder.Entity<Goleiro>().HasKey(g => g.Id);
            modelBuilder.Entity<JogadorLobby>().HasKey(j => j.Id);
            modelBuilder.Entity<UsuarioTemporario>().HasKey(u => u.Id);

            modelBuilder.Entity<Evento>()
                .HasOne(e => e.UsuarioCriador)
                .WithMany(u => u.EventosCriados)
                .HasForeignKey(e => e.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.UsuarioCriador)
                .WithMany(u => u.PartidasCriadas)
                .HasForeignKey(p => p.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Partidas)
                .WithOne(p => p.Evento)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Estatisticas)
                .WithOne(e => e.Evento)
                .HasForeignKey(e => e.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<ConfiguracaoPelada>().HasNoKey();
        }
    }
}
