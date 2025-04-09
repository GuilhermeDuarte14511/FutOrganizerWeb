using CommunityToolkit.Maui;
using FutOrganizerMobile.Application.Config;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Application.Services;
using FutOrganizerMobile.Utils;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.Maui.Audio;

namespace FutOrganizerMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome6Free-Solid-900.ttf", "FontAwesome");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Configuração da URL da API
            var baseApiUrl = "https://futorganizerapi-bxc4d0egepcuh3gx.brazilsouth-01.azurewebsites.net/api";
            builder.Services.AddSingleton(new ApiConfig
            {
                BaseUrl = baseApiUrl
            });

            // Serviços HTTP
            builder.Services.AddHttpClient<ILoginService, LoginService>(client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
            });

            builder.Services.AddHttpClient<IPartidaService, PartidaService>(client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
            });

            builder.Services.AddHttpClient<IUsuarioService, UsuarioService>(client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
            });

            // Plugin de áudio
            builder.Services.AddSingleton<IAudioManager, AudioManager>();

            // Monta o app
            var app = builder.Build();

            ServiceHelper.Init(app.Services);

            return app;
        }
    }
}
