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

    // Constructor overload with ServiceHelper
    public HomePage() : this(ServiceHelper.GetService<IPartidaService>()) { }

    // Navigate to SorteioPage for regular match creation
    private async void OnCriarSalaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SorteioPage(isRapido: false));
    }

    private async void OnMinhasSalasClicked(object sender, EventArgs e)
    {
        var usuarioId = Preferences.Get("UsuarioId", "");
        if (!Guid.TryParse(usuarioId, out Guid usuarioGuid))
        {
            await DisplayAlert("Erro", "Usuário não autenticado.", "OK");
            return;
        }

        await Navigation.PushAsync(new MinhasSalasPage(ServiceHelper.GetService<IPartidaService>()));
    }


    // Navigate to SorteioPage for fast match creation (quick mode)
    private async void OnSorteioRapidoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SorteioPage(isRapido: true));
    }

    // Navigate to HistoricoPage to view historical match data
    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoricoPage(_partidaService));
    }

    // Navigate to CronometroPage for timer functionality
    private async void OnCronometroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CronometroPage());
    }

    // Handle user logout by showing a confirmation dialog and clearing preferences
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sair", "Deseja realmente sair?", "Sim", "Cancelar");
        if (confirm)
        {
            // Remove user data from preferences
            Preferences.Remove("UsuarioId");
            Preferences.Remove("UsuarioNome");

            // Navigate back to the root page (Login page or initial screen)
            await Navigation.PopToRootAsync();
        }
    }
}
