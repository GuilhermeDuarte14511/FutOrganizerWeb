using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Services;
using FutOrganizerWeb.Domain.Interfaces;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using FutOrganizerWeb.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FutOrganizerWeb.Infrastructure.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISorteioService, SorteioService>();
            services.AddScoped<IPartidaService, PartidaService>();

            // Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ISorteioRepository, SorteioRepository>();
            services.AddScoped<IPartidaRepository, PartidaRepository>();

            return services;
        }
    }
}
