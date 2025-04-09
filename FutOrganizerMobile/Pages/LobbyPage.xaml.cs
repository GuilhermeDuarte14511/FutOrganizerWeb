using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Application.Services;
using FutOrganizerMobile.ViewModels;

namespace FutOrganizerMobile.Pages;

public partial class LobbyPage : ContentPage
{
    private readonly string _codigo;
    private readonly JogadorDTO _jogador;
    private readonly IPartidaService _partidaService;
    private HubConnection _connection;

    private ObservableCollection<string> _mensagens = new();
    private ObservableCollection<JogadorLobbyViewModel> _jogadores = new();
    private readonly HashSet<string> _digitandoUsuarios = new();
    private HashSet<string> _usuariosOnline = new();
    private List<string> _jogadoresAntigos = new();

    public LobbyPage(string codigo, JogadorDTO jogador, IPartidaService partidaService)
    {
        InitializeComponent();
        _codigo = codigo;
        _jogador = jogador;
        _partidaService = partidaService;

        lblCodigoLobby.Text = $"Código da Sala: {_codigo}";
        MensagensListView.ItemsSource = _mensagens;
        JogadoresCollection.ItemsSource = _jogadores;

        CarregarMensagensDoHistorico();
        IniciarSignalR();
        StartPollingJogadores();
    }

    private async void CarregarMensagensDoHistorico()
    {
        try
        {
            var mensagens = await _partidaService.ObterMensagensChatAsync(_codigo);
            foreach (var msg in mensagens)
            {
                var hora = msg.DataEnvio.ToString("HH:mm");
                _mensagens.Add($"{hora} - {msg.NomeUsuario}: {msg.Conteudo}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Erro ao carregar histórico: {ex.Message}");
        }
    }

    private async void IniciarSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"https://futorganizerweb20250331151702-c7euadbra2cbgphj.brazilsouth-01.azurewebsites.net/hubs/lobbychat?codigoSala={_codigo}&nome={_jogador.Nome}")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string, string>("ReceberMensagem", async (nome, mensagem, hora) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                _mensagens.Add($"{hora} - {nome}: {mensagem}");

                if (nome != _jogador.Nome)
                    await NotificationService.EnviarNotificacaoAsync($"Nova mensagem de {nome}", mensagem, "chat");
            });
        });

        _connection.On<string>("MostrarDigitando", nome =>
        {
            if (nome != _jogador.Nome)
            {
                _digitandoUsuarios.Add(nome);
                AtualizarStatusDigitando();

                Task.Delay(3000).ContinueWith(_ =>
                {
                    _digitandoUsuarios.Remove(nome);
                    MainThread.BeginInvokeOnMainThread(AtualizarStatusDigitando);
                });
            }
        });

        _connection.On<List<string>>("AtualizarUsuariosOnline", lista =>
        {
            _usuariosOnline = new HashSet<string>(lista);
            MainThread.BeginInvokeOnMainThread(AtualizarJogadores);
        });

        try
        {
            await _connection.StartAsync();
            Console.WriteLine("?? Conectado ao SignalR");
        }
        catch (Exception ex)
        {
            Console.WriteLine("? Erro ao conectar: " + ex.Message);
        }
    }

    private void AtualizarStatusDigitando()
    {
        LblDigitando.Text = _digitandoUsuarios.Count == 0
            ? string.Empty
            : $"{string.Join(", ", _digitandoUsuarios)} {(_digitandoUsuarios.Count > 1 ? "estão" : "está")} digitando...";
    }

    private async void OnEnviarMensagemClicked(object sender, EventArgs e)
    {
        var msg = MensagemEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(msg))
        {
            var mensagemSalva = await _partidaService.EnviarMensagemChatAsync(_codigo, _jogador.Nome, msg);

            if (mensagemSalva is not null)
            {
                await _connection.InvokeAsync("EnviarMensagem", _codigo, _jogador.Nome, msg);
                MensagemEntry.Text = string.Empty;
            }
        }
    }

    private async void OnMensagemDigitada(object sender, TextChangedEventArgs e)
    {
        await _connection.InvokeAsync("UsuarioDigitando", _codigo, _jogador.Nome);
        await _partidaService.AtualizarAtividadeAsync(_codigo, _jogador.Nome);
    }

    private void StartPollingJogadores()
    {
        Device.StartTimer(TimeSpan.FromSeconds(10), () =>
        {
            _ = AtualizarJogadoresAsync();
            return true;
        });
    }

    private async Task AtualizarJogadoresAsync()
    {
        try
        {
            var todos = await _partidaService.ObterJogadoresAsync(_codigo);
            var nomesAtuais = todos.Select(j => j.Nome).ToList();

            var novos = nomesAtuais.Except(_jogadoresAntigos).ToList();
            var sairam = _jogadoresAntigos.Except(nomesAtuais).ToList();

            foreach (var nome in novos)
            {
                if (nome != _jogador.Nome)
                    await NotificationService.EnviarNotificacaoAsync("Novo jogador no lobby", $"{nome} entrou na sala.", "entrou");
            }

            foreach (var nome in sairam)
            {
                if (nome != _jogador.Nome)
                    await NotificationService.EnviarNotificacaoAsync("Jogador saiu do lobby", $"{nome} saiu da sala.", "saiu");
            }

            _jogadoresAntigos = nomesAtuais;

            _jogadores.Clear();
            foreach (var j in todos)
            {
                var cor = _usuariosOnline.Contains(j.Nome) ? Colors.LimeGreen : Colors.Red;
                var atividade = $"Visto por último às {j.UltimaAtividade:HH:mm}";

                _jogadores.Add(new JogadorLobbyViewModel
                {
                    Nome = j.Nome,
                    UltimaAtividadeFormatada = atividade,
                    CorStatus = cor
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("? Erro ao atualizar jogadores: " + ex.Message);
        }
    }

    private void AtualizarJogadores()
    {
        _ = AtualizarJogadoresAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (_connection is not null)
        {
            await _connection.StopAsync();
        }
    }
}
