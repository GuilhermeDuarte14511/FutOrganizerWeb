using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Utils;
using System.Text.RegularExpressions;
using Microsoft.Maui.Storage;

namespace FutOrganizerMobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ILoginService _loginService;
    private bool _senhaVisivel = false;

    public LoginPage(ILoginService loginService)
    {
        InitializeComponent();
        _loginService = loginService;
    }

    private void OnToggleSenhaClicked(object sender, EventArgs e)
    {
        _senhaVisivel = !_senhaVisivel;
        SenhaEntry.IsPassword = !_senhaVisivel;
        ToggleSenhaBtn.Text = _senhaVisivel ? "\uf06e" : "\uf070"; // FontAwesome: eye/eye-slash
        ToggleSenhaBtn.TextColor = _senhaVisivel ? Colors.White : Colors.LightGray;
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        BtnEntrar.IsEnabled = false;
        BtnEntrar.Text = "Entrando...";

        string email = EmailEntry.Text?.Trim();
        string senha = SenhaEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            ToastHelper.ShowToast(MainLayout, "Preencha todos os campos!", Colors.Red);
            ResetarBotao();
            return;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ToastHelper.ShowToast(MainLayout, "E-mail inválido!", Colors.OrangeRed);
            ResetarBotao();
            return;
        }

        var usuario = await _loginService.LogarAsync(email, senha);

        if (usuario != null)
        {
            Preferences.Set("UsuarioId", usuario.Id.ToString());
            Preferences.Set("UsuarioNome", usuario.Nome);

            ToastHelper.ShowToast(MainLayout, $"Bem-vindo {usuario.Nome}", Colors.Green);

            var partidaService = ServiceHelper.GetService<IPartidaService>();
            await Navigation.PushAsync(new HomePage(partidaService));
        }
        else
        {
            ToastHelper.ShowToast(MainLayout, "Email ou senha inválidos", Colors.Red);
        }

        ResetarBotao();
    }

    private void ResetarBotao()
    {
        BtnEntrar.IsEnabled = true;
        BtnEntrar.Text = "Entrar";
    }
}
