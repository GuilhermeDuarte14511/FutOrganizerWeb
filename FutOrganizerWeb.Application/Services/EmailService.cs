using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateRepository _templateRepository;

        public EmailService(IConfiguration configuration, IEmailTemplateRepository templateRepository)
        {
            _configuration = configuration;
            _templateRepository = templateRepository;
        }

        public async Task EnviarBoasVindasAsync(string destinatarioNome, string destinatarioEmail, string loginUrl)
        {
            try
            {

                var client = new SendGridClient(_configuration["SendGridApiKeyFutOrganizer"]);
                var from = new EmailAddress("naoresponda_futorganizer14511@outlook.com");
                var to = new EmailAddress(destinatarioEmail, destinatarioNome);
                var template = await _templateRepository.ObterPorTipoAsync("BoasVindas");
                if (template == null)
                    throw new Exception("Template de boas-vindas não encontrado ou HTML vazio.");

                var dynamicTemplateData = new
                {
                    nome = destinatarioNome,
                    email = destinatarioEmail,
                    loginUrl = loginUrl
                };

                var msg = MailHelper.CreateSingleTemplateEmail(from, to, template.SendGridTemplateId, dynamicTemplateData);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    var error = await response.Body.ReadAsStringAsync();
                    throw new Exception($"Erro ao enviar boas-vindas. Status: {response.StatusCode}");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task EnviarRecuperacaoSenhaAsync(string destinatarioNome, string destinatarioEmail, string linkRedefinicao)
        {
            try
            {
                var client = new SendGridClient(_configuration["SendGridApiKeyFutOrganizer"]);
                var from = new EmailAddress("naoresponda_futorganizer14511@outlook.com");
                var to = new EmailAddress(destinatarioEmail, destinatarioNome);
                var template = await _templateRepository.ObterPorTipoAsync("RecuperacaoSenha");
                if (template == null)
                    throw new Exception("Template de recuperação de senha não encontrado ou HTML vazio.");

                var dynamicTemplateData = new
                {
                    nome = destinatarioNome,
                    linkRedefinicao = linkRedefinicao
                };

                var msg = MailHelper.CreateSingleTemplateEmail(from, to, template.SendGridTemplateId, dynamicTemplateData);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    var error = await response.Body.ReadAsStringAsync();
                    throw new Exception($"Erro ao enviar e-mail de recuperação. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
