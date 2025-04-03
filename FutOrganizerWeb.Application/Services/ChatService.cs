using FutOrganizerWeb.Application.DTOs;
using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Application.Interfaces.Repositories;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;

namespace FutOrganizerWeb.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;
        private readonly IPartidaRepository _partidaRepository;

        public ChatService(IChatRepository repository, IPartidaRepository partidaRepository)
        {
            _repository = repository;
            _partidaRepository = partidaRepository;
        }

        public async Task<MensagemChatDTO?> SalvarMensagemAsync(string codigoSala, string nomeUsuario, string mensagem)
        {
            var partida = await _partidaRepository.ObterPorCodigoAsync(codigoSala);
            if (partida == null) return null;

            var fusoBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var horaBrasilia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fusoBrasilia);

            var novaMensagem = new MensagemChat
            {
                PartidaId = partida.Id,
                NomeJogador = nomeUsuario,
                Conteudo = mensagem,
                DataHoraEnvio = horaBrasilia
            };

            await _repository.AdicionarMensagemAsync(novaMensagem);

            return new MensagemChatDTO
            {
                NomeUsuario = novaMensagem.NomeJogador,
                Conteudo = novaMensagem.Conteudo,
                DataEnvio = novaMensagem.DataHoraEnvio
            };
        }

        public async Task<List<MensagemChatDTO>> ObterMensagensAsync(string codigoSala)
        {
            var mensagens = await _repository.ObterMensagensPorCodigoSalaAsync(codigoSala);

            return mensagens
                .OrderBy(m => m.DataHoraEnvio)
                .Select(m => new MensagemChatDTO
                {
                    NomeUsuario = m.NomeJogador,
                    Conteudo = m.Conteudo,
                    DataEnvio = m.DataHoraEnvio
                })
                .ToList();
        }
    }
}
