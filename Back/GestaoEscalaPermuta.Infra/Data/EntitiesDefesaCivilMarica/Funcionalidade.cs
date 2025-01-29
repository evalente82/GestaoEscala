using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Funcionalidade
    {
        [Key]
        public Guid IdFuncionalidade { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = null!;

        [StringLength(255)]
        public string? Descricao { get; set; }

        // Relacionamento com Perfil
        public ICollection<PerfisFuncionalidades>? PerfisFuncionalidades { get; set; }
    }
}
