﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        protected IActionResult? Autenticado()
        {
            var usuarioId = ObterUsuarioLogado();
            if (usuarioId == Guid.Empty)
            {
                return RedirectToAction("Login", "Login");
            }

            // Returning null means the user is authenticated;
            // otherwise the method returns a redirect to the login action.
            return null;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
