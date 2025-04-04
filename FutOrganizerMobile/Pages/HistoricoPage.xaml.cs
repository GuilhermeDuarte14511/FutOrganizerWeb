using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using Microsoft.Maui.Storage;

namespace FutOrganizerMobile.Pages;

public partial class HistoricoPage : ContentPage
{
    private readonly IPartidaService _partidaService;
    private readonly List<HistoricoPartidaDto> _partidas = new();
    private int _paginaAtual = 1;
    private const int QuantidadePorPagina = 6;

    private Guid _usuarioId;

    public HistoricoPage(IPartidaService partidaService)
    {
        InitializeComponent();
        _partidaService = partidaService;

        var usuarioIdStr = Preferences.Get("UsuarioId", null);
        if (!Guid.TryParse(usuarioIdStr, out _usuarioId))
        {
            DisplayAlert("Erro", "Não foi possível obter o ID do usuário. Faça login novamente.", "OK");
            return;
        }

        LoadPartidas();
    }

    private async void LoadPartidas()
    {
        BtnMostrarMais.IsEnabled = false;
        BtnMostrarMais.Text = "Carregando...";

        var novasPartidas = await _partidaService.ObterHistoricoAsync(_usuarioId, _paginaAtual, QuantidadePorPagina);

        if (novasPartidas is not null && novasPartidas.Any())
        {
            _partidas.AddRange(novasPartidas);
            HistoricoCollection.ItemsSource = null;
            HistoricoCollection.ItemsSource = _partidas;

            _paginaAtual++;
            BtnMostrarMais.IsEnabled = true;
            BtnMostrarMais.Text = "Mostrar Mais";
        }
        else
        {
            BtnMostrarMais.IsVisible = false;
        }
    }


    private void OnMostrarMaisClicked(object sender, EventArgs e)
    {
        LoadPartidas();
    }

    private async void OnVerDetalhesClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var partidaId = button.CommandParameter?.ToString();

        await DisplayAlert("Detalhes", $"Abrir detalhes da partida {partidaId}", "OK");
        // Pode navegar para outra página futuramente
    }
}
