using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Domain.DTOs;
using FutOrganizerMobile.Utils;

namespace FutOrganizerMobile.Pages;

public partial class CadastrarUsuarioPage : ContentPage
{
    private readonly IUsuarioService _usuarioService;

    public CadastrarUsuarioPage(IUsuarioService usuarioService)
    {
        InitializeComponent();
        _usuarioService = usuarioService;
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string senha = SenhaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            ToastHelper.mostrarToast(ToastContainer, "Preencha todos os campos.", Colors.OrangeRed);
            return;
        }

        if (!IsValidEmail(email))
        {
            ToastHelper.mostrarToast(ToastContainer, "Email inválido.", Colors.OrangeRed);
            return;
        }

        if (!IsValidPassword(senha))
        {
            ToastHelper.mostrarToast(ToastContainer, "Senha fraca. Use mínimo 8 caracteres, 1 número, 1 letra e 1 símbolo.", Colors.OrangeRed);
            return;
        }

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        var request = new CadastrarUsuarioRequest
        {
            Nome = nome,
            Email = email,
            Senha = senha
        };

        var response = await _usuarioService.CadastrarUsuarioAsync(request);

        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;

        ToastHelper.mostrarToast(ToastContainer, response.Mensagem,
            response.Sucesso ? Colors.ForestGreen : Colors.OrangeRed);

        if (response.Sucesso)
        {
            NomeEntry.Text = EmailEntry.Text = SenhaEntry.Text = string.Empty;
            AtualizarRequisitosSenha("");

            // Aguarda 3 segundos e faz fade out com redirecionamento
            await Task.Delay(3000);
            await this.FadeTo(0, 500, Easing.CubicOut); // Animação de saída
            await Navigation.PopAsync(); // Volta para a tela de login
        }
    }

    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPassword(string senha)
    {
        return senha.Length >= 8 &&
               senha.Any(char.IsDigit) &&
               senha.Any(char.IsLetter) &&
               senha.Any(ch => !char.IsLetterOrDigit(ch));
    }

    private void SenhaEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        AtualizarRequisitosSenha(e.NewTextValue ?? "");
    }

    private void AtualizarRequisitosSenha(string senha)
    {
        RegraMinCaracteres.TextColor = senha.Length >= 8 ? Colors.Green : Colors.Red;
        RegraNumero.TextColor = senha.Any(char.IsDigit) ? Colors.Green : Colors.Red;
        RegraLetra.TextColor = senha.Any(char.IsLetter) ? Colors.Green : Colors.Red;
        RegraEspecial.TextColor = senha.Any(ch => !char.IsLetterOrDigit(ch)) ? Colors.Green : Colors.Red;
    }
}
