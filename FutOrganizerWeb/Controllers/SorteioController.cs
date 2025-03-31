using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FutOrganizerWeb.Controllers
{
    public class SorteioController : Controller
    {
        private readonly ISorteioService _sorteioService;
        private readonly IPartidaService _partidaService;

        public SorteioController(ISorteioService sorteioService, IPartidaService partidaService)
        {
            _sorteioService = sorteioService;
            _partidaService = partidaService;
        }


        // GET: /Sorteio
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] SorteioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return Unauthorized("Usuário não autenticado.");

            request.UsuarioCriadorId = usuarioId;

            var sorteioId = await _sorteioService.CriarSorteioAsync(request);
            return Ok(new { sucesso = true, sorteioId });
        }


        // POST: /Sorteio/Resortear
        [HttpPost]
        public async Task<IActionResult> Resortear([FromBody] ResortearRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var sorteioId = await _sorteioService.ResortearAsync(request.SorteioId, request.Times);
            return Ok(new { sucesso = true, sorteioId });
        }

        [HttpGet]
        public IActionResult Historico()
        {
            return View();
        }


    }
}
