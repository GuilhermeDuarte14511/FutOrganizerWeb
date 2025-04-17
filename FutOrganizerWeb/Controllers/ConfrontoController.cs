using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Controllers
{
    public class ConfrontoController : BaseController
    {
        private readonly IConfrontoService _confrontoService;
        private readonly IPartidaRepository _partidaRepository;
        private readonly ISorteioRepository _sorteioRepository;

        public ConfrontoController(
            IConfrontoService confrontoService,
            IPartidaRepository partidaRepository,
            ISorteioRepository sorteioRepository)
        {
            _confrontoService = confrontoService;
            _partidaRepository = partidaRepository;
            _sorteioRepository = sorteioRepository;
        }

        [HttpGet("Confronto/Partida/{codigo}")]
        public async Task<IActionResult> Index(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return RedirectToAction("Index", "Home");

            var partida = await _partidaRepository.ObterPorCodigoAsync(codigo);
            if (partida == null)
                return RedirectToAction("Index", "Home");

            var sorteio = partida.Sorteios
                .OrderByDescending(s => s.Data)
                .FirstOrDefault();

            if (sorteio == null)
                return RedirectToAction("Index", "Sorteio", new { codigo });

            var times = sorteio.Times.Select(t => new TimeConfrontoDTO
            {
                Id = t.Id,
                Nome = t.Nome,
                Cor = t.CorHex,
                Jogadores = t.Jogadores.Select(j => new JogadorDTO
                {
                    Id = j.Id,
                    Nome = j.Nome
                }).ToList()
            }).ToList();

            ViewBag.Codigo = codigo;
            ViewBag.SorteioId = sorteio.Id;
            ViewBag.Times = times;

            return View(sorteio.Id); // SorteioId como Model (GUID)
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Guid sorteioId, Guid timeAId, Guid timeBId)
        {
            await _confrontoService.CriarConfrontoAsync(sorteioId, timeAId, timeBId);
            return Json(new { sucesso = true, mensagem = "Confronto criado com sucesso!" });
        }

        [HttpGet]
        public async Task<IActionResult> ListaPorSorteio(Guid sorteioId)
        {
            var confrontos = await _confrontoService.ObterConfrontosPorSorteioAsync(sorteioId);
            return Json(confrontos); // Retorna List<ConfrontoDTO>
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarGol(Guid confrontoId, Guid jogadorId)
        {
            await _confrontoService.AdicionarGolAsync(confrontoId, jogadorId);
            return Json(new { sucesso = true, mensagem = "Gol registrado!" });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarAssistencia(Guid confrontoId, Guid jogadorId)
        {
            await _confrontoService.AdicionarAssistenciaAsync(confrontoId, jogadorId);
            return Json(new { sucesso = true, mensagem = "Assistência registrada!" });
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarPlacar(Guid confrontoId, int golsA, int golsB)
        {
            await _confrontoService.AtualizarPlacarAsync(confrontoId, golsA, golsB);
            return Json(new { sucesso = true, mensagem = "Placar atualizado!" });
        }
    }
}
