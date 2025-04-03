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

        var resultado = await _loginService.LogarAsync(email, senha);

        if (resultado.sucesso)
        {
            // Se o login for bem-sucedido
            await DisplayAlert("Login", $"Bem-vindo {resultado.usuarioId}", "OK");
            // Redirecionar para outra página ou realizar outra ação
        }
        else
        {
            // Caso contrário, exibe a mensagem de erro
            await DisplayAlert("Erro", resultado.mensagem, "OK");
        }
    }
}