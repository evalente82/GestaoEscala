using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Funcionalidade
    {
        public int IdFuncionalidade { get; set; }

        [StringLength(100)]
        public string NmNome { get; set; } = null!;

        [StringLength(200)]
        public string NmDescricao { get; set; } = null!;

        // Relacionamentos
        public ICollection<PerfilFuncionalidade>? PerfisFuncionalidades { get; set; }
    }
}
