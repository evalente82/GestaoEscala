using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class CriacaoEscalaExtraCargo
    {
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdCargo { get; set; }

        // Propriedades de navegação de volta para as entidades principais
        public virtual CriacaoEscalaExtra CriacaoEscalaExtra { get; set; }
        public virtual Cargo Cargo { get; set; }
    }
}
