using FutOrganizerWeb.Application.Interfaces;
using FutOrganizerWeb.Domain.Entities;
using FutOrganizerWeb.Domain.Interfaces_Repositories;
using Microsoft.EntityFrameworkCore;

namespace FutOrganizerWeb.Infrastructure.Persistence.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly FutOrganizerDbContext _context;

        public EmailTemplateRepository(FutOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<EmailTemplate?> ObterPorTipoAsync(string tipo)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Tipo.ToLower() == tipo.ToLower());
        }
    }
}
