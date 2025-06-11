using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class EscalaExtra
    {
        [Key]
        public Guid IdEscalaExtra { get; set; }

        [Required]
        public Guid IdCriacaoEscalaExtra { get; set; }        

        [Required]
        public Guid IdFuncionario { get; set; }

        [Required]
        public DateTime DtServico { get; set; }

        public DateTime DtCriacao { get; set; } = DateTime.Now;
    }

}
