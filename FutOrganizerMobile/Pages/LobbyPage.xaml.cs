using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Models;
using FutOrganizerMobile.ViewModels;
using FutOrganizerMobile.Application.Services;
using System.Text.Json;

namespace FutOrganizerMobile.Pages;

public partial class LobbyPage : ContentPage
{
    private readonly string _codigo;
    private readonly JogadorDTO _jogador;
    private readonly IPartidaService _partidaService;
    private HubConnection _connection;

    private readonly ObservableCollection<string> _mensagens = new();
    private readonly ObservableCollection<JogadoresAgrupados> _jogadoresAgrupados = new();
    private readonly HashSet<string> _digitandoUsuarios = new();
    private HashSet<string> _usuariosOnline = new();
    private HashSet<string> _jogadoresAntigos = new();
    private readonly HashSet<string> _notificadosEntraram = new();
    private readonly HashSet<string> _notificadosSairam = new();
    private bool _atualizandoJogadores = false;
    private bool _sorteioJaDetectado = false; // ? controle do polling

    public LobbyPage(string codigo, JogadorDTO jogador, IPartidaService partidaService)
    {
        InitializeComponent();
        _codigo = codigo;
        _jogador = jogador;
        _partidaService = partidaService;

        lblCodigoLobby.Text = $"Código da Sala: {_codigo}";
        MensagensListView.ItemsSource = _mensagens;
        JogadoresCollection.ItemsSource = _jogadoresAgrupados;

        CarregarMensagensDoHistorico();
        IniciarSignalR();
        StartPollingJogadores();
    }

    #region Mensagens
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
    #endregion

    #region Digitando
    private async void OnMensagemDigitada(object sender, TextChangedEventArgs e)
    {
        await _connection.InvokeAsync("UsuarioDigitando", _codigo, _jogador.Nome);
        await _partidaService.AtualizarAtividadeAsync(_codigo, _jogador.Nome);
    }

    private void AtualizarStatusDigitando()
    {
        LblDigitando.Text = _digitandoUsuarios.Count == 0
            ? string.Empty
            : $"{string.Join(", ", _digitandoUsuarios)} {(_digitandoUsuarios.Count > 1 ? "estão" : "está")} digitando...";
    }
    #endregion

    #region SignalR
    private async void IniciarSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"https://futorganizerweb20250331151702-c7euadbra2cbgphj.brazilsouth-01.azurewebsites.net/hubs/lobbychat?codigoSala={_codigo}&nome={_jogador.Nome}")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string, string>("ReceberMensagem", async (nome, mensagem, hora) =>
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _mensagens.Add($"{hora} - {nome}: {mensagem}");
                if (nome != _jogador.Nome)
                    _ = NotificationService.EnviarNotificacaoAsync($"Nova mensagem de {nome}", mensagem, "chat");
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
            Console.WriteLine("? Conectado ao SignalR");
        }
        catch (Exception ex)
        {
            Console.WriteLine("? Erro ao conectar: " + ex.Message);
        }
    }
    #endregion

    #region Sorteio
    private void StartPollingSorteio()
    {
        Device.StartTimer(TimeSpan.FromSeconds(8), () =>
        {
            if (_sorteioJaDetectado)
                return false;

            _ = VerificarSorteioAsync();
            return true;
        });
    }

    private async Task VerificarSorteioAsync()
    {
        try
        {
            var (sucesso, sorteio) = await _partidaService.VerificarSorteioAsync(_codigo);
            if (!sucesso || sorteio == null)
                return;

            _sorteioJaDetectado = true;
            await CarregarSorteioAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Erro ao verificar sorteio: {ex.Message}");
        }
    }

    private async Task CarregarSorteioAsync()
    {
        try
        {
            var (sucesso, sorteio) = await _partidaService.VerificarSorteioAsync(_codigo);
            if (!sucesso || sorteio == null)
                return;

            TimesSorteadosLayout.Children.Clear();

            foreach (var time in sorteio.Times.OrderBy(t => t.Nome))
            {
                var contemJogadorAtual = time.Jogadores.Any(j => j.Id == _jogador.Id);

                var frame = new Frame
                {
                    BackgroundColor = Color.FromArgb(time.CorHex),
                    CornerRadius = 12,
                    Padding = 10,
                    HasShadow = true,
                    BorderColor = Colors.White
                };

                var stack = new VerticalStackLayout();
                stack.Children.Add(new Label
                {
                    Text = time.Nome,
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18
                });

                if (!string.IsNullOrEmpty(time.Goleiro))
                {
                    stack.Children.Add(new Label
                    {
                        Text = $"?? {time.Goleiro} (Gol)",
                        TextColor = Colors.White,
                        FontSize = 14
                    });
                }

                foreach (var jogador in time.Jogadores)
                {
                    stack.Children.Add(new Label
                    {
                        Text = jogador.Nome,
                        TextColor = jogador.Id == _jogador.Id ? Colors.LimeGreen : Colors.White,
                        FontAttributes = jogador.Id == _jogador.Id ? FontAttributes.Bold : FontAttributes.None
                    });
                }

                frame.Content = stack;
                TimesSorteadosLayout.Children.Add(frame);
            }

            TimesSorteadosLayout.IsVisible = true;

            var btnSair = this.FindByName<Button>("BtnSairSala");
            if (btnSair != null)
                btnSair.IsVisible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("? Erro ao carregar sorteio: " + ex.Message);
        }
    }
    #endregion

    #region Jogadores
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
        if (_atualizandoJogadores) return;
        _atualizandoJogadores = true;

        try
        {
            var jogadoresAtuais = await _partidaService.ObterJogadoresAsync(_codigo);
            var nomesAtuais = jogadoresAtuais.Select(j => j.Nome).ToHashSet();

            var novos = nomesAtuais.Except(_jogadoresAntigos).ToList();
            var sairam = _jogadoresAntigos.Except(nomesAtuais).ToList();

            foreach (var nome in novos)
            {
                if (nome != _jogador.Nome && !_notificadosEntraram.Contains(nome))
                {
                    await NotificationService.EnviarNotificacaoAsync("Novo jogador no lobby", $"{nome} entrou na sala.", "entrou");
                    _notificadosEntraram.Add(nome);
                    _notificadosSairam.Remove(nome);
                }
            }

            foreach (var nome in sairam)
            {
                if (nome != _jogador.Nome && !_notificadosSairam.Contains(nome))
                {
                    await NotificationService.EnviarNotificacaoAsync("Jogador saiu do lobby", $"{nome} saiu da sala.", "saiu");
                    _notificadosSairam.Add(nome);
                    _notificadosEntraram.Remove(nome);
                }
            }

            _jogadoresAntigos = nomesAtuais;

            var distintos = jogadoresAtuais
                .GroupBy(j => j.Nome)
                .Select(g => g.First());

            var online = distintos
                .Where(j => _usuariosOnline.Contains(j.Nome))
                .OrderBy(j => j.Nome)
                .Select(j => new JogadorLobbyViewModel
                {
                    Nome = j.Nome,
                    UltimaAtividadeFormatada = $"Visto por último às {j.UltimaAtividade:HH:mm}",
                    CorStatus = Colors.LimeGreen
                });

            var offline = distintos
                .Where(j => !_usuariosOnline.Contains(j.Nome))
                .OrderBy(j => j.Nome)
                .Select(j => new JogadorLobbyViewModel
                {
                    Nome = j.Nome,
                    UltimaAtividadeFormatada = $"Visto por último às {j.UltimaAtividade:HH:mm}",
                    CorStatus = Colors.Red
                });

            _jogadoresAgrupados.Clear();
            _jogadoresAgrupados.Add(new JogadoresAgrupados($"?? Online ({online.Count()})", online));
            _jogadoresAgrupados.Add(new JogadoresAgrupados($"?? Offline ({offline.Count()})", offline));
        }
        catch (Exception ex)
        {
            Console.WriteLine("? Erro ao atualizar jogadores: " + ex.Message);
        }
        finally
        {
            _atualizandoJogadores = false;
        }
    }

    private void AtualizarJogadores()
    {
        _ = AtualizarJogadoresAsync();
    }
    #endregion

    #region Ciclo de Vida
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _notificadosEntraram.Clear();
        _notificadosSairam.Clear();

        _sorteioJaDetectado = false;
        StartPollingSorteio();
        await CarregarSorteioAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        _notificadosEntraram.Clear();
        _notificadosSairam.Clear();

        if (_connection is not null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }

    private async void OnSairSalaClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Sair da Sala", "Tem certeza que deseja sair da sala?", "Sim", "Cancelar");
        if (confirmar)
        {
            await Navigation.PopAsync();
        }
    }
    #endregion
}