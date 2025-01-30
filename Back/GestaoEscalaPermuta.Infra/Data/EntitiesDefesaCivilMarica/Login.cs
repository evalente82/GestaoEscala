using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Login
    {
        [Key] // Define a propriedade como chave primária
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Gera automaticamente um valor único
        public Guid Id { get; set; }

        [Required]
        public string Usuario { get; set; }

        public string SenhaHash { get; set; }

        public string Perfil { get; set; } // Exemplo: "Administrador", "Guarda Vidas"
    }
}
