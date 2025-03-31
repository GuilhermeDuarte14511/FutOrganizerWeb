using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class ResortearRequest
    {
        public Guid SorteioId { get; set; }
        public List<TimeRequest> Times { get; set; } = new();
    }
}
