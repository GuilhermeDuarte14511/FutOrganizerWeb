using FutOrganizerMobile.Application.Config;
using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Application.Services;
using Microsoft.Extensions.Logging;

namespace FutOrganizerMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome6Free-Solid-900.ttf", "FontAwesome");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Definir diretamente a URL base da API
            builder.Services.AddSingleton(new ApiConfig
            {
                BaseUrl = "https://localhost:7159/api"
            });

            builder.Services.AddHttpClient<ILoginService, LoginService>(client =>
            {
                // Usar a URL base diretamente
                client.BaseAddress = new Uri("https://localhost:7159/api");
            });

            return builder.Build();
        }
    }
}
