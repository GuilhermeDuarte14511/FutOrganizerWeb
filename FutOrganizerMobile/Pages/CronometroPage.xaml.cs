using Microsoft.Maui.Controls;
using System;
using System.Timers;
using Plugin.Maui.Audio;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Pages
{
    public partial class CronometroPage : ContentPage
    {
        private int tempoTotalSegundos = 0;
        private System.Timers.Timer cronometro;
        private bool emContagem = false;
        private IAudioPlayer _alarmePlayer;
        private bool alertaAtivo = false;

        public CronometroPage()
        {
            InitializeComponent();
            CarregarAlarme();
            AtualizarDisplay();
        }

        private async void CarregarAlarme()
        {
            try
            {
                var audioService = AudioManager.Current;
                var fileStream = await FileSystem.OpenAppPackageFileAsync("mixkit-classic-alarm-995.mp3");
                _alarmePlayer = audioService.CreatePlayer(fileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar alarme: " + ex.Message);
            }
        }

        private void OnIniciarClicked(object sender, EventArgs e)
        {
            if (emContagem) return;

            if (tempoTotalSegundos == 0)
            {
                int.TryParse(MinutosEntry.Text, out int minutos);
                int.TryParse(SegundosEntry.Text, out int segundos);
                tempoTotalSegundos = minutos * 60 + segundos;
            }

            if (tempoTotalSegundos <= 0) return;

            emContagem = true;
            alertaAtivo = false;

            cronometro = new System.Timers.Timer(1000);
            cronometro.Elapsed += OnTimerElapsed;
            cronometro.Start();
        }

        private void OnPararClicked(object sender, EventArgs e)
        {
            PararCronometro();
            ResetarEstiloAlerta();
        }

        private void OnResetarClicked(object sender, EventArgs e)
        {
            PararCronometro();
            tempoTotalSegundos = 0;
            MinutosEntry.Text = "";
            SegundosEntry.Text = "";
            TempoLabel.Scale = 1;
            ResetarEstiloAlerta();
            AtualizarDisplay();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            tempoTotalSegundos--;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                AtualizarDisplay();

                if (tempoTotalSegundos <= 0 && !alertaAtivo)
                {
                    alertaAtivo = true;
                    PararCronometro();

                    // Ativar vibração
                    try
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromSeconds(2));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao vibrar: " + ex.Message);
                    }

                    _alarmePlayer?.Play();
                    await AnimarAlertaAsync();
                }
            });
        }

        private async Task AnimarAlertaAsync()
        {
            // Mudar visual do relógio
            TempoDisplayBorder.BackgroundColor = Colors.Red;
            TempoLabel.TextColor = Colors.White;

            while (alertaAtivo)
            {
                await TempoLabel.ScaleTo(1.2, 200);
                await TempoLabel.ScaleTo(1.0, 200);
            }
        }

        private void AtualizarDisplay()
        {
            int minutos = tempoTotalSegundos / 60;
            int segundos = tempoTotalSegundos % 60;
            TempoLabel.Text = $"{minutos:D2}:{segundos:D2}";
        }

        private void ResetarEstiloAlerta()
        {
            TempoDisplayBorder.BackgroundColor = Color.FromArgb("#1e1e2f");
            TempoLabel.TextColor = Colors.White;
        }

        private void PararCronometro()
        {
            if (cronometro != null)
            {
                cronometro.Stop();
                cronometro.Dispose();
                cronometro = null;
            }

            emContagem = false;
            alertaAtivo = false;
        }
    }
}
