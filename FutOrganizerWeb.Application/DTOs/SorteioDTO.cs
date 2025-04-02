﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Application.DTOs
{
    public class SorteioDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public List<TimeDTO> Times { get; set; } = new();
    }

}
