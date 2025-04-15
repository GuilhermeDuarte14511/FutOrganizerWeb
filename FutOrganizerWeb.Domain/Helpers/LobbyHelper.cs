using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutOrganizerWeb.Domain.Helpers
{
    public static class LobbyHelper
    {
        public static string ObterIdentificadorJogador(Guid? usuarioAutenticadoId, string nome)
        {
            return usuarioAutenticadoId.HasValue && usuarioAutenticadoId != Guid.Empty
                ? usuarioAutenticadoId.ToString()
                : nome;
        }
    }
}
