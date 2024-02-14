using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica
{
    public class PerfilFuncionalidade
    {
        public int IdPerfil { get; set; }
        public int IdFuncionalidade { get; set; }

        // Relacionamentos
        public Perfil Perfil { get; set; } = null!;
        public Funcionalidade Funcionalidade { get; set; } = null!;
    }
}
