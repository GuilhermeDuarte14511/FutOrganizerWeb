using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace FutOrganizerMobile;

public partial class App : IApplication
{
    private readonly ILoginService _loginService;
    private readonly IPartidaService _partidaService;

    public App(ILoginService loginService, IPartidaService partidaService)
    {
        InitializeComponent();

        _loginService = loginService;
        _partidaService = partidaService;

        // Recupera código de sala pendente (se existir)
        var codigoSalaPendente = Preferences.Get("CodigoSalaPendente", null);

        // Define página inicial
        var loginPage = new LoginPage(_loginService, codigoSalaPendente);
        var navPage = new NavigationPage(loginPage)
        {
            BarBackgroundColor = Color.FromArgb("#121212"),
            BarTextColor = Colors.White
        };

        MainPage = navPage;

        // Executa solicitação de permissão após navegação inicial
        loginPage.NavigatedTo += async (_, _) => await SolicitarPermissaoNotificacoes();

        // Ouve clique nas notificações
        LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);

        if (uri != null && uri.AbsolutePath.StartsWith("/Lobby/", StringComparison.OrdinalIgnoreCase))
        {
            string codigo = uri.AbsolutePath.Split("/Lobby/")[1];
            var usuarioId = Preferences.Get("UsuarioId", null);

            if (!string.IsNullOrEmpty(usuarioId))
            {
                await MainPage.Navigation.PushAsync(new SorteioPage(false, codigo));
            }
            else
            {
                Preferences.Set("CodigoSalaPendente", codigo);
                await MainPage.Navigation.PushAsync(new LoginPage(_loginService, codigo));
            }
        }
    }

    private async Task SolicitarPermissaoNotificacoes()
    {
        try
        {
            // Permissão de notificação
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();

                if (status == PermissionStatus.Denied)
                {
                    bool abrirConfig = await MainPage.DisplayAlert(
                        "Notificações desativadas",
                        "Você negou as permissões de notificação. Deseja abrir as configurações do app para ativá-las?",
                        "Abrir configurações",
                        "Cancelar");

#if ANDROID
                    if (abrirConfig)
                    {
                        try
                        {
                            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                            intent.SetData(Android.Net.Uri.Parse($"package:{Android.App.Application.Context.PackageName}"));
                            intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                            Android.App.Application.Context.StartActivity(intent);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Erro ao abrir configurações: {ex.Message}");
                        }
                    }
#endif
                }
            }

            // Solicitar vibração separadamente
            await SolicitarPermissaoVibracao();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao solicitar permissões: {ex.Message}");
        }
    }

    private async Task SolicitarPermissaoVibracao()
    {
        if (!Preferences.ContainsKey("VibracaoAtivada"))
        {
            bool ativarVibracao = await MainPage.DisplayAlert(
                "Deseja ativar vibração?",
                "Ao receber notificações do lobby/chat, o celular poderá vibrar.",
                "Ativar vibração",
                "Não vibrar");

            Preferences.Set("VibracaoAtivada", ativarVibracao);
        }
    }


    private async void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        if (!e.IsTapped || e.Request?.ReturningData is not string data) return;

        if (data is "chat" or "entrou" or "saiu")
        {
            var codigo = Preferences.Get("CodigoSalaPendente", null);
            if (!string.IsNullOrWhiteSpace(codigo))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    MainPage.Navigation.PushAsync(new SorteioPage(false, codigo)));
            }
        }
    }
}
