using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class FuncionarioPerfil
    {
        public int IdFuncionario { get; set; }
        public int IdPerfil { get; set; }

        // Relacionamentos
        public Funcionario Funcionario { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
