using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FutOrganizerMobile.Pages;

public partial class MinhasSalasPage : ContentPage
{
    private readonly IPartidaService _partidaService;
    private ObservableCollection<SalaViewModel> _salas;
    private List<SalaViewModel> _todasSalas; // lista completa para aplicar filtros
    private int _paginaAtual = 1;
    private const int TamanhoPagina = 10;

    public ObservableCollection<SalaViewModel> Salas
    {
        get => _salas;
        set
        {
            _salas = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> StatusOptions { get; set; }
    public string SelectedStatus { get; set; }

    public MinhasSalasPage(IPartidaService partidaService)
    {
        InitializeComponent();
        _partidaService = partidaService;
        _todasSalas = new List<SalaViewModel>();
        Salas = new ObservableCollection<SalaViewModel>();
        StatusOptions = new ObservableCollection<string> { "Todas", "Abertas", "Finalizadas" };
        SelectedStatus = StatusOptions[0];
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarSalas();
    }

    private async Task CarregarSalas()
    {
        var usuarioId = Preferences.Get("UsuarioId", "");
        if (Guid.TryParse(usuarioId, out Guid usuarioGuid))
        {
            btnCarregarMais.Text = "Carregando...";
            btnCarregarMais.IsEnabled = false;

            var novasSalas = await _partidaService.ObterSalasAsync(usuarioGuid, _paginaAtual, TamanhoPagina);

            if (novasSalas.Any())
            {
                foreach (var sala in novasSalas)
                    _todasSalas.Add(sala);

                _paginaAtual++;
                btnCarregarMais.IsVisible = novasSalas.Count == TamanhoPagina;
            }
            else
            {
                btnCarregarMais.IsVisible = false;
            }

            AplicarFiltro(); // renderiza na tela com base no status atual
            btnCarregarMais.Text = "Carregar Mais";
            btnCarregarMais.IsEnabled = true;
        }
    }

    private async void OnLoadMoreClicked(object sender, EventArgs e)
    {
        await CarregarSalas();
    }

    private void OnStatusChanged(object sender, EventArgs e)
    {
        AplicarFiltro();
    }

    private void OnBuscaChanged(object sender, TextChangedEventArgs e)
    {
        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        var buscaTexto = buscaEntry.Text?.ToLower()?.Trim() ?? "";

        var filtradas = _todasSalas
            .Where(s =>
                (SelectedStatus == "Todas" ||
                 (SelectedStatus == "Abertas" && !s.Finalizada) ||
                 (SelectedStatus == "Finalizadas" && s.Finalizada)) &&
                (string.IsNullOrEmpty(buscaTexto) || s.Codigo.ToLower().Contains(buscaTexto)))
            .ToList();

        Salas.Clear();
        foreach (var sala in filtradas)
        {
            Salas.Add(sala);
        }
    }

    private async void OnCopiarLinkClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.Parent is Layout layout)
        {
            var entry = layout.Children.OfType<Entry>().FirstOrDefault();
            if (entry != null && !string.IsNullOrWhiteSpace(entry.Text))
            {
                await Clipboard.SetTextAsync(entry.Text);
                ToastHelper.ShowToast(toastContainer, "Link copiado com sucesso!", Colors.Green);
            }
            else
            {
                ToastHelper.ShowToast(toastContainer, "Link inválido!", Colors.DarkRed);
            }
        }
    }


    private async void OnDetalhesClicked(object sender, EventArgs e)
    {
        if (sender is Button botao &&
            botao.BindingContext is SalaViewModel sala &&
            !string.IsNullOrEmpty(sala.Codigo))
        {
            if (sala.Finalizada)
            {
                // Sala já possui sorteio — futura tela de detalhes
                await ToastHelper.ShowToastAsync(toastContainer, "Funcionalidade de visualizar sorteio em breve!", Colors.Orange);
            }
            else
            {
                await Navigation.PushAsync(new SorteioPage(isRapido: false, codigoSalaExistente: sala.Codigo));
            }
        }
        else
        {
            await ToastHelper.ShowToastAsync(toastContainer, "Erro ao acessar sala.", Colors.Red);
        }
    }

}
