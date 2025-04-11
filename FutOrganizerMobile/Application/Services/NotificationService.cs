using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace FutOrganizerMobile.Application.Services
{
    public static class NotificationService
    {
        private static readonly string DefaultChannelId = "default";

        /// <summary>
        /// Envia uma notificação local ao dispositivo, com verificação de permissão em tempo de execução (Android 13+).
        /// </summary>
        public static async Task EnviarNotificacaoAsync(string titulo, string mensagem, string? dadoRetorno = null)
        {
#if ANDROID
            var granted = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (!granted)
            {
                granted = await LocalNotificationCenter.Current.RequestNotificationPermission();
                if (!granted)
                    return;
            }
#endif
            var requisicao = new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 9999), // ID aleatório
                Title = titulo,
                Description = mensagem,
                ReturningData = dadoRetorno ?? string.Empty,
                Android = new AndroidOptions
                {
                    ChannelId = DefaultChannelId,
                    Priority = AndroidPriority.High,
                    VisibilityType = AndroidVisibilityType.Public,
                    AutoCancel = true,
                    IconSmallName = new AndroidIcon("ic_launcher"),
                    VibrationPattern = Preferences.Get("VibracaoAtivada", false)
                     ? new long[] { 0, 300, 200, 300 } : null
                }
            };

            await LocalNotificationCenter.Current.Show(requisicao);
        }

        /// <summary>
        /// Cancela todas as notificações exibidas ou pendentes.
        /// </summary>
        public static void CancelarTodas()
        {
            LocalNotificationCenter.Current.CancelAll();
        }

        /// <summary>
        /// Cancela uma notificação específica pelo ID.
        /// </summary>
        public static void CancelarPorId(int id)
        {
            LocalNotificationCenter.Current.Cancel(id);
        }
    }
}
