using CommunityToolkit.Maui;
using FutOrganizerMobile.Application.Config;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Application.Services;
using FutOrganizerMobile.Utils;
using Microsoft.Extensions.Logging;
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
            builder.Services.AddSingleton(new ApiConfig
            {
                BaseUrl = "https://localhost:7159/api"
            });

            // Serviços
            builder.Services.AddHttpClient<ILoginService, LoginService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7159/api");
            });

            builder.Services.AddHttpClient<IPartidaService, PartidaService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7159/api");
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