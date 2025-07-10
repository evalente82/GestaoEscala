using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public enum TipoFeriado
    {
        Nacional,
        Estadual,
        Municipal
    }
    [Table("Feriado")]
    public class Feriado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(100)]
        public string Descricao { get; set; }

        [Required]
        public TipoFeriado Tipo { get; set; }
    }
}