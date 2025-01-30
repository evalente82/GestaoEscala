using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class CargoPerfis
    {
        public Guid IdCargo { get; set; }
        public Guid IdPerfil { get; set; }
        public Cargo Cargo { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
