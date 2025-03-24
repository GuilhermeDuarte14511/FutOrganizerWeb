using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace FutOrganizerWeb.Application.Services
{
    public class AuthService : IAuthService
    {
        public string GerarHash(string senha)
        {
            // Gera o hash usando BCrypt
            var hashGerado = BCrypt.Net.BCrypt.HashPassword(senha);

            // Apenas para debug
            Debug.WriteLine("🔐 SENHA ORIGINAL: " + senha);
            Debug.WriteLine("🧂 HASH GERADO: " + hashGerado);

            return hashGerado;
        }

        public bool VerificarHash(string senhaDigitada, string hashArmazenado)
        {
            var resultado = BCrypt.Net.BCrypt.Verify(senhaDigitada, hashArmazenado);
            var hashGerado = BCrypt.Net.BCrypt.HashPassword(senhaDigitada);

            // Apenas para debug
            Debug.WriteLine("🔎 SENHA DIGITADA: " + senhaDigitada);
            Debug.WriteLine("📦 HASH ARMAZENADO: " + hashArmazenado);
            Debug.WriteLine("✅ VERIFICADO: " + resultado);

            return resultado;
        }

        public void SalvarUsuarioNaSessao(HttpContext context, Usuario usuario)
        {
            context.Session.SetString("UsuarioId", usuario.Id.ToString());
            context.Session.SetString("UsuarioNome", usuario.Nome);
            context.Session.SetString("UsuarioEmail", usuario.Email);
        }

    }
}
