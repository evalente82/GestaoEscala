using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Perfil
    {
        public int IdPerfil { get; set; }

        [StringLength(100)]
        public string NmNome { get; set; } = null!;

        // Relacionamentos
        public ICollection<FuncionarioPerfil>? FuncionariosPerfis { get; set; }
        public ICollection<PerfilFuncionalidade>? PerfisFuncionalidades { get; set; }
    }
}
