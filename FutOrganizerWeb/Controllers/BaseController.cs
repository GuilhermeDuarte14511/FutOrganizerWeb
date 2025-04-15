using Microsoft.AspNetCore.Mvc;
using System;

namespace FutOrganizerWeb.Controllers
{
    public class BaseController : Controller
    {
        protected Guid ObterUsuarioLogado()
        {
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdString) || !Guid.TryParse(usuarioIdString, out var usuarioId))
                return Guid.Empty;

            return usuarioId;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
