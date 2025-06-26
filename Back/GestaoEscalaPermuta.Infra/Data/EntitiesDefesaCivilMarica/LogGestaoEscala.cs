

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    [Table("LogGestaoEscala")]
    public class LogGestaoEscala
    {
        [Key]
        public long Id { get; set; }

        public DateTimeOffset DataOcorrencia { get; set; }

        [Required]
        [StringLength(255)]
        public string UsuarioResponsavel { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAcao { get; set; }

        [StringLength(100)]
        public string? EntidadeAfetada { get; set; }

        public string? IdEntidadeAfetada { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValoresAntigos { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValoresNovos { get; set; }

        [Column(TypeName = "jsonb")]
        public string? MetadadosAdicionais { get; set; }
    }
}
