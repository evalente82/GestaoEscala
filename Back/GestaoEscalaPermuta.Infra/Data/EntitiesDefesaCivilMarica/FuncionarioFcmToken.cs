using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Entities
{
    [Table("FuncionarioFcmTokens", Schema = "public")]
    public class FuncionarioFcmToken
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("IdFuncionario")]
        public Guid IdFuncionario { get; set; }

        [Column("FcmToken")]
        [Required]
        [MaxLength(255)]
        public string FcmToken { get; set; }

        [Column("DataRegistro")]
        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

        [Column("DataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        [ForeignKey("IdFuncionario")]
        public virtual Funcionario Funcionario { get; set; }
    }
}
