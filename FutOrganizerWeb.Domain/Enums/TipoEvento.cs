using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Enums
{
    public enum TipoEvento
    {
        [Display(Name = "Pelada")]
        Pelada = 1,

        [Display(Name = "Campeonato Eliminatório")]
        CampeonatoEliminatorio = 2,

        [Display(Name = "Torneio em Grupo")]
        TorneioGrupo = 3,

        [Display(Name = "Copa dos Amigos")]
        CopaDosAmigos = 4,

        [Display(Name = "Treinamento")]
        Treinamento = 5,

        [Display(Name = "Outro")]
        Outro = 99
    }

}
