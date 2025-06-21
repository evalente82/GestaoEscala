using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class CriacaoEscalaExtra
    {
        [Key]
        public Guid IdCriacaoEscalaExtra { get; set; }

        [StringLength(255)]
        [Unicode(false)]
        public string NmEscalaExtra { get; set; } = null!;

        [Required]
        public Guid IdSetor { get; set; }

        [Required]
        public DateTime DtEscalaExtra { get; set; }

        [Required]
        public DateTime DtAbertura { get; set; }

        [Required]
        public DateTime DtFechamento { get; set; }       

        [Required]
        public Guid IdFuncionario { get; set; }

        public bool IsAtivo { get; set; } = true;

        public DateTime DtCriacao { get; set; } = DateTime.Now;
        public int QtdVagas { get; set; }

    }
}
