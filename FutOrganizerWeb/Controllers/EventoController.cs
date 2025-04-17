using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Controllers
{
    public class EventoController : BaseController
    {
        private readonly IEventoService _eventoService;

        public EventoController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterUsuarioLogado();
            if (usuarioId == Guid.Empty)
                return RedirectToAction("Login", "Login");

            var eventos = await _eventoService.ListarEventosDoUsuarioAsync(usuarioId);
            return View(eventos);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] EventoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { sucesso = false, mensagem = "Dados inválidos." });

            dto.UsuarioCriadorId = ObterUsuarioLogado();
            if (dto.UsuarioCriadorId == Guid.Empty)
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não autenticado." });

            var eventoId = await _eventoService.CriarEventoAsync(dto);
            return Ok(new { sucesso = true, mensagem = "Evento criado com sucesso!", eventoId });
        }

        [HttpGet("/Evento/Detalhes/{id}")]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            var evento = await _eventoService.ObterPorIdAsync(id);
            if (evento == null)
                return NotFound("Evento não encontrado");

            return View(evento);
        }

        [HttpDelete]
        public async Task<IActionResult> Remover(Guid id)
        {
            var usuarioId = ObterUsuarioLogado();
            if (usuarioId == Guid.Empty)
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não autenticado." });

            var sucesso = await _eventoService.RemoverEventoAsync(id, usuarioId);
            if (!sucesso)
                return NotFound(new { sucesso = false, mensagem = "Evento não encontrado ou não pertence a você." });

            return Ok(new { sucesso = true, mensagem = "Evento removido com sucesso!" });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] EventoDTO dto)
        {
            var usuarioId = ObterUsuarioLogado();
            if (usuarioId == Guid.Empty)
                return Unauthorized(new { sucesso = false, mensagem = "Usuário não autenticado." });

            dto.UsuarioCriadorId = usuarioId;
            var atualizado = await _eventoService.EditarEventoAsync(dto);
            if (!atualizado)
                return NotFound(new { sucesso = false, mensagem = "Evento não encontrado ou não pertence a você." });

            return Ok(new { sucesso = true, mensagem = "Evento atualizado com sucesso!" });
        }
    }
}
