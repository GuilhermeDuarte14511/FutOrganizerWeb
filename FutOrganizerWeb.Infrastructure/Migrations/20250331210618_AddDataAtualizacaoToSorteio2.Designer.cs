﻿// <auto-generated />
using System;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FutOrganizerWeb.Infrastructure.Migrations
{
    [DbContext(typeof(FutOrganizerDbContext))]
    [Migration("20250331210618_AddDataAtualizacaoToSorteio2")]
    partial class AddDataAtualizacaoToSorteio2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EstatisticaJogador", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Assistencias")
                        .HasColumnType("int");

                    b.Property<int>("Defesas")
                        .HasColumnType("int");

                    b.Property<Guid>("EventoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Gols")
                        .HasColumnType("int");

                    b.Property<string>("NomeJogador")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventoId");

                    b.ToTable("Estatisticas");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.ConfiguracaoPelada", b =>
                {
                    b.Property<int>("JogadoresPorTime")
                        .HasColumnType("int");

                    b.Property<bool>("TemGoleiroFixo")
                        .HasColumnType("bit");

                    b.ToTable("ConfiguracoesPelada");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Evento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UsuarioCriadorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioCriadorId");

                    b.ToTable("Eventos");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Goleiro", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Goleiros");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Jogador", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TimeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TimeId");

                    b.ToTable("Jogadores");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.JogadorLobby", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DataEntrada")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PartidaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PartidaId");

                    b.ToTable("JogadoresLobby");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Partida", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CodigoLobby")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataHora")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("EventoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<string>("Local")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<Guid?>("UsuarioCriadorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("EventoId");

                    b.HasIndex("UsuarioCriadorId");

                    b.ToTable("Partidas");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Time", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CorHex")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GoleiroId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SorteioId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GoleiroId");

                    b.HasIndex("SorteioId");

                    b.ToTable("Times");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Usuario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SenhaHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Sorteio", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataAtualizacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PartidaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PartidaId");

                    b.ToTable("Sorteios");
                });

            modelBuilder.Entity("EstatisticaJogador", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Evento", "Evento")
                        .WithMany("Estatisticas")
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Evento");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Evento", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Usuario", "UsuarioCriador")
                        .WithMany("EventosCriados")
                        .HasForeignKey("UsuarioCriadorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("UsuarioCriador");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Jogador", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Time", null)
                        .WithMany("Jogadores")
                        .HasForeignKey("TimeId");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.JogadorLobby", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Partida", "Partida")
                        .WithMany("JogadoresLobby")
                        .HasForeignKey("PartidaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partida");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Partida", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Evento", "Evento")
                        .WithMany("Partidas")
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FutOrganizerWeb.Domain.Entities.Usuario", "UsuarioCriador")
                        .WithMany("PartidasCriadas")
                        .HasForeignKey("UsuarioCriadorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Evento");

                    b.Navigation("UsuarioCriador");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Time", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Goleiro", "Goleiro")
                        .WithMany()
                        .HasForeignKey("GoleiroId");

                    b.HasOne("Sorteio", null)
                        .WithMany("Times")
                        .HasForeignKey("SorteioId");

                    b.Navigation("Goleiro");
                });

            modelBuilder.Entity("Sorteio", b =>
                {
                    b.HasOne("FutOrganizerWeb.Domain.Entities.Partida", "Partida")
                        .WithMany("Sorteios")
                        .HasForeignKey("PartidaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partida");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Evento", b =>
                {
                    b.Navigation("Estatisticas");

                    b.Navigation("Partidas");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Partida", b =>
                {
                    b.Navigation("JogadoresLobby");

                    b.Navigation("Sorteios");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Time", b =>
                {
                    b.Navigation("Jogadores");
                });

            modelBuilder.Entity("FutOrganizerWeb.Domain.Entities.Usuario", b =>
                {
                    b.Navigation("EventosCriados");

                    b.Navigation("PartidasCriadas");
                });

            modelBuilder.Entity("Sorteio", b =>
                {
                    b.Navigation("Times");
                });
#pragma warning restore 612, 618
        }
    }
}
