using FutOrganizerMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerMobile.Models
{
    public class JogadoresAgrupados : ObservableCollection<JogadorLobbyViewModel>
    {
        public string NomeGrupo { get; set; }

        public JogadoresAgrupados(string nomeGrupo, IEnumerable<JogadorLobbyViewModel> jogadores) : base(jogadores)
        {
            NomeGrupo = nomeGrupo;
        }
    }
}
