using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [Required]
        public bool Ativo { get; set; } = true;

        public Guid? IdFuncionario { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Perfil { get; set; } // Ex.: "Administrador", "Guarda Vidas"
    }
}
