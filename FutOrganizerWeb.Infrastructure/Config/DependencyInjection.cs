using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Interfaces_Repositories;
using FutOrganizerWeb.Application.Services;
using FutOrganizerWeb.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FutOrganizerWeb.Infrastructure.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();

            // Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            return services;
        }
    }
}
