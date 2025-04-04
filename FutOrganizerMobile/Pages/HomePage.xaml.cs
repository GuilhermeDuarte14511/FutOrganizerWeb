using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Pages;
using FutOrganizerMobile.Utils;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace FutOrganizerMobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly IPartidaService _partidaService;

    public HomePage(IPartidaService partidaService)
    {
        InitializeComponent();
        _partidaService = partidaService;
    }

    public HomePage() : this(ServiceHelper.GetService<IPartidaService>()) { }

    private async void OnCriarSalaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SorteioPage(isRapido: false));
    }

    private async void OnMinhasSalasClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Minhas Salas", "Funcionalidade em construção.", "OK");
    }

    private async void OnSorteioRapidoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SorteioPage(isRapido: true));
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoricoPage(_partidaService));
    }

    private async void OnCronometroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CronometroPage());
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sair", "Deseja realmente sair?", "Sim", "Cancelar");
        if (confirm)
        {
            Preferences.Remove("UsuarioId");
            Preferences.Remove("UsuarioNome");

            await Navigation.PopToRootAsync();
        }
    }
}