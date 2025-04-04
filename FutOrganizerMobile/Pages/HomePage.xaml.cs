using FutOrganizerMobile.Pages;

namespace FutOrganizerMobile.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnCriarSalaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Criar Sala", "Funcionalidade em construção.", "OK");
            // Ex: await Navigation.PushAsync(new CriarSalaPage());
        }

        private async void OnMinhasSalasClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Minhas Salas", "Funcionalidade em construção.", "OK");
        }

        private async void OnSorteioRapidoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sorteio Rápido", "Funcionalidade em construção.", "OK");
        }

        private async void OnHistoricoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Histórico", "Funcionalidade em construção.", "OK");
        }

        private async void OnCronometroClicked(object sender, EventArgs e)
        {
            // Redireciona para a página de cronômetro
            await Navigation.PushAsync(new CronometroPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sair", "Deseja realmente sair?", "Sim", "Cancelar");
            if (confirm)
            {
                // Volta para tela de login
                await Navigation.PopToRootAsync();
            }
        }
    }
}
