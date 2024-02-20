using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Log
    {
        public int LogID { get; set; }

        public int IdFuncionario { get; set; }
        public Funcionario Funcionario { get; set; } = null!;

        public int IdFuncionalidade { get; set; }

        [StringLength(200)]
        public Funcionalidade Funcionalidade { get; set; } = null!;

        [StringLength(500)]
        public string Inputs { get; set; } = null!;

        [StringLength(500)]
        public string TipoErro { get; set; } = null!;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}
