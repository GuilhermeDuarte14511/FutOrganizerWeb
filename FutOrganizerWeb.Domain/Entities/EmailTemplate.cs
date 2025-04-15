using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Entities
{
    public class EmailTemplate
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string SendGridTemplateId { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? EmailHtml { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }

}
