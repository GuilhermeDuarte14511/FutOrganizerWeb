using FutOrganizerMobile.Application.Interfaces.Services;

namespace FutOrganizerMobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ILoginService _loginService;
    private bool _senhaVisivel = false;

    // Construtor com injeção de dependência
    public LoginPage(ILoginService loginService)
    {
        InitializeComponent();
        _loginService = loginService;
    }

    private void OnToggleSenhaClicked(object sender, EventArgs e)
    {
        _senhaVisivel = !_senhaVisivel;
        SenhaEntry.IsPassword = !_senhaVisivel;
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string senha = SenhaEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Erro", "Preencha todos os campos!", "OK");
            return;
        }

        var usuario = await _loginService.LogarAsync(email, senha);

        if (usuario != null)
        {
            await DisplayAlert("Login", $"Bem-vindo {usuario.Nome} ({usuario.Id})", "OK");

            await Navigation.PushAsync(new HomePage());

        }
        else
        {
            await DisplayAlert("Erro", "Email ou senha inválidos", "OK");
        }
    }

}