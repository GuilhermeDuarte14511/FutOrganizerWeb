using System.Collections.ObjectModel;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerMobile.Utils;
using System.Globalization;

namespace FutOrganizerMobile.Pages;

public partial class SorteioPage : ContentPage
{
    private readonly bool _modoRapido;
    private readonly ObservableCollection<TimeModel> _times = new();
    private readonly IPartidaService _partidaService;
    private string? _codigo;

    public SorteioPage(bool isRapido = false, string? codigoSalaExistente = null)
    {
        InitializeComponent();
        _modoRapido = isRapido;
        _partidaService = ServiceHelper.GetService<IPartidaService>();

        GoalkeeperEditor.IsEnabled = false;
        BtnSortearGoleiros.IsVisible = false;
        TeamsCollection.ItemsSource = _times;

        PlayerNamesEditor.TextChanged += (s, e) => AtualizarContagem();
        HasFixedGoalkeeperCheck.CheckedChanged += (s, e) => ToggleInputGoleiro();

        // Se foi passado um código existente, já setamos ele
        if (!string.IsNullOrEmpty(codigoSalaExistente))
        {
            _codigo = codigoSalaExistente;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_modoRapido)
        {
            LinkBorder.IsVisible = false;
        }
        else if (!string.IsNullOrEmpty(_codigo))
        {
            LinkCompartilhavel.Text = $"https://seudominio.com/Lobby/{_codigo}";
            LinkBorder.IsVisible = true;
            await CarregarJogadoresLobby();
        }
        else
        {
            var usuarioIdStr = Preferences.Get("UsuarioId", "");
            if (!Guid.TryParse(usuarioIdStr, out Guid usuarioId))
            {
                ToastHelper.ShowToast(ToastContainer, "Usuário não autenticado.", Colors.Red);
                return;
            }

            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                if (location == null)
                {
                    ToastHelper.ShowToast(ToastContainer, "Não foi possível obter a localização.", Colors.Red);
                    return;
                }

                var latitude = location.Latitude;
                var longitude = location.Longitude;
                string endereco = await AppHelper.ObterEnderecoPorCoordenadasAsync(latitude, longitude);

                var request = new CriarSalaRequest
                {
                    UsuarioId = usuarioId,
                    Latitude = latitude.ToString("F7", CultureInfo.InvariantCulture),
                    Longitude = longitude.ToString("F7", CultureInfo.InvariantCulture),
                    Local = endereco
                };

                _codigo = await _partidaService.CriarSalaAsync(request);

                if (_codigo == null)
                {
                    ToastHelper.ShowToast(ToastContainer, "Erro ao criar sala.", Colors.Red);
                    return;
                }

                LinkCompartilhavel.Text = $"https://seudominio.com/Lobby/{_codigo}";
                LinkBorder.IsVisible = true;

                await CarregarJogadoresLobby();
            }
            catch (Exception ex)
            {
                ToastHelper.ShowToast(ToastContainer, $"Erro ao obter localização: {ex.Message}", Colors.Red);
            }
        }
    }

    private async Task CarregarJogadoresLobby()
    {
        try
        {
            var partida = await _partidaService.ObterPorCodigoAsync(_codigo);
            if (partida?.JogadoresLobby != null)
            {
                PlayerNamesEditor.Text = string.Join("\n", partida.JogadoresLobby.Select(j => j.Nome));
                AtualizarContagem();
            }
        }
        catch
        {
            ToastHelper.ShowToast(ToastContainer, "Erro ao buscar jogadores do lobby.", Colors.Red);
        }
    }

    private void AtualizarContagem()
    {
        var jogadores = ObterJogadores();
        PlayerCountLabel.Text = $"{jogadores.Count} jogador(es) adicionado(s)";
        var listaNumerada = jogadores.Select((nome, index) => $"{index + 1} - {nome}").ToList();
        PlayerListCollection.ItemsSource = listaNumerada;
    }

    private List<string> ObterJogadores() =>
        PlayerNamesEditor.Text?.Split('\n').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new();

    private List<string> ObterGoleiros() =>
        GoalkeeperEditor.Text?.Split('\n').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new();

    private void ToggleInputGoleiro()
    {
        GoalkeeperEditor.IsEnabled = HasFixedGoalkeeperCheck.IsChecked;
        GoalkeeperEditor.Text = string.Empty;
        BtnSortearGoleiros.IsVisible = HasFixedGoalkeeperCheck.IsChecked;
    }

    private void OnCopiarLinkClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(LinkCompartilhavel.Text))
        {
            Clipboard.SetTextAsync(LinkCompartilhavel.Text);
            ToastHelper.ShowToast(ToastContainer, "Link copiado com sucesso!", Colors.Green);
        }
    }

    private void OnSortearGoleirosClicked(object sender, EventArgs e)
    {
        ToastHelper.ShowToast(ToastContainer, "Função ainda em construção", Colors.Orange);
    }

    private async void OnGerarTimesClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(PlayersPerTeamEntry.Text, out int porTime) || porTime <= 0)
        {
            ToastHelper.ShowToast(ToastContainer, "Informe um número válido de jogadores por time.", Colors.Orange);
            return;
        }

        var jogadores = ObterJogadores();
        if (jogadores.Count == 0)
        {
            ToastHelper.ShowToast(ToastContainer, "Adicione jogadores antes de gerar os times!", Colors.Orange);
            return;
        }

        var timesCount = (int)Math.Ceiling(jogadores.Count / (double)porTime);

        try
        {
            LoadingOverlay.IsVisible = true;
            LoadingTextLabel.Text = "Gerando times...";
            await Task.Delay(300);

            jogadores = jogadores.OrderBy(_ => Guid.NewGuid()).ToList();

            _times.Clear();
            var timesRequest = new List<TimeRequest>();

            for (int i = 0; i < timesCount; i++)
            {
                LoadingTextLabel.Text = $"Gerando Time {i + 1}...";
                await Task.Delay(300);

                var jogadoresTime = jogadores.Skip(i * porTime).Take(porTime).ToList();
                var cor = GerarCorHexAleatoria();

                _times.Add(new TimeModel
                {
                    Nome = $"Time {i + 1}",
                    CorHex = cor,
                    Jogadores = jogadoresTime,
                    Goleiro = null
                });

                timesRequest.Add(new TimeRequest
                {
                    Nome = $"Time {i + 1}",
                    CorHex = cor,
                    Jogadores = jogadoresTime,
                    Goleiro = null
                });
            }

            var usuarioId = Preferences.Get("UsuarioId", "");
            if (!Guid.TryParse(usuarioId, out Guid usuarioGuid))
            {
                ToastHelper.ShowToast(ToastContainer, "Usuário não autenticado.", Colors.Red);
                return;
            }

            LoadingTextLabel.Text = "Salvando sorteio...";
            await Task.Delay(500);

            var request = new SorteioRequest
            {
                NomeSorteio = $"Sorteio - {DateTime.Now:HH:mm:ss}",
                CodigoLobby = _codigo ?? "",
                UsuarioCriadorId = usuarioGuid,
                Times = timesRequest
            };

            var sucesso = await _partidaService.CriarSorteioAsync(request);

            if (sucesso)
                ToastHelper.ShowToast(ToastContainer, "Sorteio salvo com sucesso!", Colors.Green);
            else
                ToastHelper.ShowToast(ToastContainer, "Erro ao salvar o sorteio!", Colors.Red);
        }
        catch (Exception ex)
        {
            ToastHelper.ShowToast(ToastContainer, $"Erro: {ex.Message}", Colors.Red);
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private string GerarCorHexAleatoria()
    {
        var bytes = new byte[3];
        Random.Shared.NextBytes(bytes);
        return "#" + string.Join("", bytes.Select(b => b.ToString("X2")));
    }
}
