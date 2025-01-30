using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class PerfisFuncionalidades
    {
        public Guid IdPerfil { get; set; }
        public Guid IdFuncionalidade { get; set; }
        public Perfil Perfil { get; set; } = null!;
        public Funcionalidade Funcionalidade { get; set; } = null!;
    }
}
